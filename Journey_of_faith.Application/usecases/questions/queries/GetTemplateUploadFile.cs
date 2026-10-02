using MediatR;

namespace Journey_of_faith.Application.usecases.questions.queries;


public class GetTemplateUploadFileQuery : IRequest<string>
{
    
}


public class GetTemplateUploadFileHandler : IRequestHandler<GetTemplateUploadFileQuery, string>
{
    public async Task<string> Handle(GetTemplateUploadFileQuery getTemplateUploadFileQuery, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", "excel_upload_template.xlsx");
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File template is not found {nameof(filePath)}");
        }
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);

        return Convert.ToBase64String(fileBytes);
    }
}