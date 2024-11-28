using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using TaxifyApp.Models;

namespace TaxifyApp.Services
{
    public class FirestoreService
    {
        private FirestoreDb db;
        public string StatusMessage;

        public FirestoreService()
        {
            SetupFireStore().Wait(); // เรียกตั้งค่าการเชื่อมต่อ Firestore
        }

        // ตั้งค่า Firestore
        private async Task SetupFireStore()
        {
            if (db == null)
            {
                try
                {
                    var stream = await FileSystem.OpenAppPackageFileAsync("taxify-dx212-firebase-adminsdk-lrquk-727e74ba73.json");
                    using (var reader = new StreamReader(stream))
                    {
                        var contents = reader.ReadToEnd();
                        db = new FirestoreDbBuilder
                        {
                            ProjectId = "taxify-dx212",
                            JsonCredentials = contents
                        }.Build();
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error setting up Firestore: {ex.Message}";
                    throw;
                }
            }
        }

        // เพิ่มข้อมูลลงใน Firestore
        public async Task InsertCalculation(CalculateModel calculation)
        {
            try
            {
                await SetupFireStore();

                // เพิ่มข้อมูลลงใน Collection "Calculations"
                await db.Collection("Calculations").AddAsync(calculation);

                StatusMessage = "Calculation successfully added!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                throw;
            }
        }

        // อัปเดตข้อมูลใน Firestore
        public async Task UpdateCalculation(string id, CalculateModel calculation)
        {
            try
            {
                await SetupFireStore();

                // อัปเดตข้อมูลในเอกสารที่มี ID ระบุ
                var docRef = db.Collection("Calculations").Document(id);
                await docRef.SetAsync(calculation, SetOptions.Overwrite);

                StatusMessage = "Calculation successfully updated!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                throw;
            }
        }

        // ลบข้อมูลใน Firestore
        public async Task DeleteCalculation(string id)
        {
            try
            {
                await SetupFireStore();

                // อ้างอิงเอกสารตาม ID และลบ
                var docRef = db.Collection("Calculations").Document(id);
                await docRef.DeleteAsync();

                StatusMessage = "Calculation successfully deleted!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                throw;
            }
        }

        // ดึงข้อมูลทั้งหมดจาก Firestore
        public async Task<List<CalculateModel>> GetAllCalculations()
        {
            try
            {
                await SetupFireStore();

                // ดึงข้อมูลทั้งหมดจาก Collection "Calculations"
                var data = await db.Collection("Calculations").GetSnapshotAsync();

                var calculations = data.Documents.Select(doc =>
                {
                    // แปลงข้อมูลเอกสารเป็น CalculateModel
                    var calculation = doc.ConvertTo<CalculateModel>();
                    return calculation;
                }).ToList();

                return calculations;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                throw;
            }
        }

        // ดึงข้อมูลโดย ID
        public async Task<CalculateModel> GetCalculationById(string id)
        {
            try
            {
                await SetupFireStore();

                // ดึงเอกสารตาม ID
                var docRef = db.Collection("Calculations").Document(id);
                var snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    // แปลงข้อมูลเอกสารเป็น CalculateModel
                    var calculation = snapshot.ConvertTo<CalculateModel>();
                    return calculation;
                }

                StatusMessage = "Document not found!";
                return null;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                throw;
            }
        }
    }
}