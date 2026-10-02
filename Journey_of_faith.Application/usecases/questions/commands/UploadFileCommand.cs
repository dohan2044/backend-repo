using System.Net.WebSockets;
using System.Text.Json;
using Journey_of_faith.Application.common.services;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using OfficeOpenXml;

namespace Journey_of_faith.Application.usecases.questions.commands;


public class UploadFileCommand : IRequest<bool>
{
    public byte[] FileBytes { get; set; }
}


public class UploadFileHandler : IRequestHandler<UploadFileCommand, bool>
{
    private readonly IQuestionRepository questionRepository;
    private readonly IQuestionQueries questionQueries;
    public UploadFileHandler(IQuestionRepository questionRepository, IQuestionQueries questionQueries)
    {
        this.questionRepository = questionRepository;
        this.questionQueries = questionQueries;
    }
    public async Task<bool> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(command.FileBytes);
        using (var package = new ExcelPackage(stream))
        {
            // lấy ra sheet được gửi lên
            var worksheet = package.Workbook.Worksheets[0];

            int totalRows = worksheet.Dimension.End.Row;
            int totalColumns = worksheet.Dimension.End.Column;

            // lấy ra tất cả các header xem đùng format hay chưa 
            var headersRange = worksheet.Cells[1, 1, 1, totalColumns];
            var expected = ColumnsValidQuestion();

            // lặp từ 1 tới 12(số cột mẫu), sau đó lấy ra các tên của cột
            var actualHeaders = Enumerable.Range(1, expected.Length)
                                    .Select(col => worksheet.Cells[1, col]?.Text?.Trim())
                                    .ToList();
            if (!expected.SequenceEqual(actualHeaders, StringComparer.OrdinalIgnoreCase))
            {
                throw new BadRequestException("Thứ tự Cột không đúng định dạng, vui lòng kiểm tra lại");
            }
            List<string> errorList = new();
            var dicCategory = await this.GetQuestionCateogries();
            var dicLevel = await this.GetQuestionLevel();
            var dicType = await this.GetQuestionType();
            // tiền hành lấy dữ liệu 
            var (insertQuestion, categoriesName) = await GetListQuestionInsert(worksheet, dicType, dicLevel);
            // lấy các danh mục có sẵn để đối chiếu
            IEnumerable<string> categoriesDb = dicCategory.Keys.ToList();
            IEnumerable<string> categoriesNotExists = categoriesName.Except(categoriesDb);

            // nếu có tên danh mục mới thì tiến hành tạo mới danh mục với tên danh mục được nhận vào
            if (categoriesNotExists.Any())
            {
                var dataObject = JsonSerializer.Serialize<List<QuestionCategory>>
                    (MapObject.MapNameToObject(categoriesNotExists.ToList()));
                await this.questionRepository.InsertMultipleCategories(dataObject);

                dicCategory = await this.GetQuestionCateogries();
            }

            foreach(var question in insertQuestion)
            {
                if(!dicCategory.TryGetValue(question.CategoryName, out var categoryId))
                {
                    throw new BadRequestException($"Không tìm thấy danh mục '{question.CategoryName}'");

                }
                question.CategoryId = categoryId;
            }
            var jsonValue = JsonSerializer.Serialize<List<CreateQuestionCommand>>(insertQuestion);

            var result = await this.questionRepository.InsertBulkQuestionAsync(jsonValue);
            if (!result)
            {
                throw new BadRequestException("Insert thất bại");
            }

            return result;
        }
    }
    private async Task<(List<CreateQuestionCommand>, List<string>)> GetListQuestionInsert(
        ExcelWorksheet worksheet,
        Dictionary<string, int> dicType,
        Dictionary<string, int> dicLevel)
    {

        List<CreateQuestionCommand> insertQuestion = new();
        List<string> categoriesName = new List<string>();

        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
        {
            var questionContent = worksheet.Cells[row, 1]?.Text?.Trim();
            var typeName = worksheet.Cells[row, 2]?.Text?.Trim();
            var categoryName = worksheet.Cells[row, 3]?.Text?.Trim();
            var levelName = worksheet.Cells[row, 4]?.Text?.Trim();
            categoriesName.Add(categoryName);
            var (list, check) = ValidateTypeAndLevelQuestion(dicType, typeName, dicLevel, levelName);
            if (list.Count > 0 || list.Any() && !check)
            {
                throw new BadRequestException(string.Join(",", list.Select(e => e)));
            }
            var question = new CreateQuestionCommand
            {
                QuestionContent = questionContent,
                TypeId = dicType[typeName],
                LevelId = dicLevel[levelName],
                CategoryName = categoryName,
                Items = new List<CreateAnswerCommand>()
            };
            for (int col = 5; col <= worksheet.Dimension.End.Column; col += 2)
            {
                var content = worksheet.Cells[row, col]?.Text?.Trim();
                var correctMark = worksheet.Cells[row, col + 1]?.Text?.Trim();

                // bỏ qua nếu đáp án trống (không phải câu hỏi nào cũng đủ 4 đáp án)
                if (string.IsNullOrEmpty(content))
                    continue;

                question.Items.Add(new CreateAnswerCommand
                {
                    Content = content,
                    IsCorrect = !string.IsNullOrEmpty(correctMark),
                });
            }
            insertQuestion.Add(question);
        }

        return (insertQuestion, categoriesName);
    }

    private async Task<Dictionary<string, int>> GetQuestionCateogries()
    {
        var categories = await this.questionQueries.GetQuestionCategoriesAsync();
        return categories
                    .ToDictionary(e => e.Name, e => e.Id);
    }
    private async Task<Dictionary<string, int>> GetQuestionType()
    {
        var categories = await this.questionQueries.GetQuestionTypesAsync();
        return categories
                    .ToDictionary(e => e.Name, e => e.Id);
    }
    private async Task<Dictionary<string, int>> GetQuestionLevel()
    {
        var categories = await this.questionQueries.GetLevelsAsync();
        return categories
                    .ToDictionary(e => e.Name, e => e.Id);
    }

    private (List<string>, bool) ValidateTypeAndLevelQuestion(
        Dictionary<string, int> listTypes,
        string currentType,
        Dictionary<string, int> listLevels,
        string currentLevel
    )
    {
        List<string> validMiss = new List<string>();
        if (!listTypes.TryGetValue(currentType, out var typeId))
        {
            validMiss.Add($"Kiểu câu hỏi không hợp lệ, vui lòng kiểm tra lại file mẫu");
        }
        if (!listLevels.TryGetValue(currentLevel, out var levelId))
        {
            validMiss.Add($"Tên độ khó không hợp lệ, Vui lòng kiểm tra lại.");
        }
        if (validMiss.Any())
            return (validMiss, false);
        return
            (new List<string>(), true);
    }
    private static string[] ColumnsValidQuestion()
    {
        return new[]
        {
           "Nội dung câu hỏi", "Loại câu hỏi", "Danh mục", "Độ khó","Đáp án 1", "Đúng 1", "Đáp án 2", "Đúng 2",
            "Đáp án 3", "Đúng 3", "Đáp án 4", "Đúng 4"
        };
    }

}
