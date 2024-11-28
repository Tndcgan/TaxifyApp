using System;
using Google.Cloud.Firestore;

namespace TaxifyApp.Models;

[FirestoreData]
public class CalculateModel
{
     [FirestoreProperty(ConverterType = typeof(DecimalConverter))]
    public decimal Exempt { get; set; }

     [FirestoreProperty(ConverterType = typeof(DecimalConverter))]
    public decimal Income { get; set; }

     [FirestoreProperty(ConverterType = typeof(DecimalConverter))]
    public decimal TaxAmount { get; set; }

    public string TaxYear { get; set; }

    [FirestoreProperty]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // เพิ่มเวลาที่บันทึกข้อมูล
}


public class DecimalConverter : IFirestoreConverter<decimal>
{
    public object ToFirestore(decimal value) => value.ToString(); // Store as a string
    
    public decimal FromFirestore(object value)
    {
        return value is string str && decimal.TryParse(str, out var result)
            ? result
            : throw new InvalidOperationException("Invalid decimal format.");
    }
}