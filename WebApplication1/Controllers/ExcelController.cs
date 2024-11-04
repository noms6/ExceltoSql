using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel; // HSSF için
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Genel Excel dosyalarý için

[Route("api/[controller]")]
[ApiController]
public class ExcelController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExcelController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("saveExcelData")]
    public async Task<IActionResult> SaveExcelData(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Bir dosya yükleyin.");
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0; // Akýþý baþa sar

            IWorkbook workbook;
            // Dosya uzantýsýna göre uygun workbook tipini belirle
            if (file.FileName.EndsWith(".xlsx"))
            {
                workbook = new XSSFWorkbook(stream); // Doðru sýnýfý kullan
            }
            else if (file.FileName.EndsWith(".xls"))
            {
                workbook = new HSSFWorkbook(stream); // HSSF, .xls için
            }
            else
            {
                return BadRequest("Desteklenmeyen dosya formatý.");
            }

            ISheet sheet = workbook.GetSheetAt(0); // Ýlk sayfayý al

            var students = new List<Student>();

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++) // Ýlk satýr baþlýk olduðu için 1'den baþlýyoruz
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    var student = new Student
                    {
                        Name = row.GetCell(0).ToString(),
                        Age = (int)row.GetCell(1).NumericCellValue // Yaþ sayýsal deðeri al
                    };
                    students.Add(student);
                }
            }

            _context.Students.AddRange(students);
            await _context.SaveChangesAsync();
            return Ok("Veriler baþarýyla kaydedildi.");
        }
    }

}
