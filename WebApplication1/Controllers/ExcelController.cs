using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel; // HSSF i�in
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // Genel Excel dosyalar� i�in

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
            return BadRequest("Bir dosya y�kleyin.");
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0; // Ak��� ba�a sar

            IWorkbook workbook;
            // Dosya uzant�s�na g�re uygun workbook tipini belirle
            if (file.FileName.EndsWith(".xlsx"))
            {
                workbook = new XSSFWorkbook(stream); // Do�ru s�n�f� kullan
            }
            else if (file.FileName.EndsWith(".xls"))
            {
                workbook = new HSSFWorkbook(stream); // HSSF, .xls i�in
            }
            else
            {
                return BadRequest("Desteklenmeyen dosya format�.");
            }

            ISheet sheet = workbook.GetSheetAt(0); // �lk sayfay� al

            var students = new List<Student>();

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++) // �lk sat�r ba�l�k oldu�u i�in 1'den ba�l�yoruz
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null)
                {
                    var student = new Student
                    {
                        Name = row.GetCell(0).ToString(),
                        Age = (int)row.GetCell(1).NumericCellValue // Ya� say�sal de�eri al
                    };
                    students.Add(student);
                }
            }

            _context.Students.AddRange(students);
            await _context.SaveChangesAsync();
            return Ok("Veriler ba�ar�yla kaydedildi.");
        }
    }

}
