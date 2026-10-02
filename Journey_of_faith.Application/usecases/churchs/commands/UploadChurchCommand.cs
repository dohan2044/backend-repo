// using Journey_of_faith.Domain.interfaces;
// using MediatR;
// using OfficeOpenXml;

// namespace Journey_of_faith.Application.usecases.churchs.commands;


// public class UploadChurchCommand : IRequest<bool>
// {
//     public byte[] FileContent { get; set; }
// }

// public class UploadChurchHandler : IRequestHandler<UploadChurchCommand, bool>
// {
//     private readonly IChurchRepository _repository;
//     public UploadChurchHandler(IChurchRepository repository)
//     {
//         _repository = repository;
//     }


//     public async Task<bool> Handle(UploadChurchCommand command, CancellationToken cancellationToken)
//     {
//         using var stream = new MemoryStream(command.FileContent);
//         using(var package = new ExcelPackage(stream))
//         {
//             var worksheet = package.Workbook.Worksheets.First();
//             var columnHeader = new {"Tên nhà thờ", "Địa chỉ"}
//         }
//     }
// }


