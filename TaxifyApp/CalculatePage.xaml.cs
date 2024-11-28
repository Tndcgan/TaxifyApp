using Microsoft.Maui.Controls;
using TaxifyApp.Models;
using TaxifyApp.Services;

namespace TaxifyApp
{
    public partial class CalculatePage : ContentPage
    {
        private string selectedTaxYear;
        private decimal netIncome;
        private decimal totalRelief;
        private decimal taxToPay;


        public CalculatePage()
        {
            InitializeComponent();
        }

        // เมื่อกดเลือกปีภาษี
        private void OnSelectYearButtonClicked(object sender, EventArgs e)
        {
            taxYearRadioGroup.IsVisible = !taxYearRadioGroup.IsVisible;
            selectYearButton.Text = taxYearRadioGroup.IsVisible ? "ซ่อนปีภาษี" : "เลือกปีภาษี";
        }

        // เมื่อเลือกปีภาษี
        private void OnTaxYearChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked)
            {
                selectedTaxYear = radioButton.Value.ToString();
                selectYearButton.Text = $"ปีภาษีที่เลือก: {selectedTaxYear}";
                taxYearRadioGroup.IsVisible = false;
            }
        }

        // เมื่อกดคำนวณ
        private void OnCalculateClicked(object sender, EventArgs e)
        {
            if (!decimal.TryParse(incomeEntry.Text, out decimal income) || income <= 0)
            {
                DisplayAlert("ข้อผิดพลาด", "กรุณากรอกรายได้ที่ถูกต้อง", "ตกลง");
                return;
            }

            decimal totalRelief = 0;

            // ลดหย่อนส่วนตัว
            if (personalReliefSwitch.IsToggled)
                totalRelief += 60000;

            // ลดหย่อนคู่สมรส
            if (spouseReliefSwitch.IsToggled)
                totalRelief += 60000;

            // ลดหย่อนบุตรในวัยเรียน
            if (int.TryParse(childrenInSchoolEntry.Text, out int childrenInSchool))
                totalRelief += childrenInSchool * 30000;

            // ลดหย่อนบุตรเกิดตั้งแต่ปี 2561
            if (int.TryParse(childrenBornAfter2018Entry.Text, out int childrenBornAfter2018))
                totalRelief += childrenBornAfter2018 * 60000;

            // ค่าฝากครรภ์หรือคลอดบุตร
            if (decimal.TryParse(pregnancyExpenseEntry.Text, out decimal pregnancyExpense))
                totalRelief += Math.Min(pregnancyExpense, 60000);

            // ค่าอุปการะพ่อแม่
            if (int.TryParse(parentsSupportEntry.Text, out int parents))
                totalRelief += parents * 30000;

            // ค่าอุปการะผู้พิการ
            if (int.TryParse(disabledSupportEntry.Text, out int disabled))
                totalRelief += disabled * 60000;

            // ลดหย่อนกลุ่มประกัน เงินออม และการลงทุน
            if (decimal.TryParse(healthInsuranceEntry.Text, out decimal healthInsurance))
                totalRelief += Math.Min(healthInsurance, 25000);

            if (decimal.TryParse(lifeInsuranceSpouseEntry.Text, out decimal lifeInsuranceSpouse))
                totalRelief += Math.Min(lifeInsuranceSpouse, 10000);

            if (decimal.TryParse(healthInsuranceParentsEntry.Text, out decimal healthInsuranceParents))
                totalRelief += Math.Min(healthInsuranceParents, 15000);

            if (decimal.TryParse(pensionInsuranceEntry.Text, out decimal pensionInsurance))
            {
                decimal pensionLimit = Math.Min(income * 0.15M, 200000);
                totalRelief += Math.Min(pensionInsurance, pensionLimit);
            }

            if (decimal.TryParse(providentFundEntry.Text, out decimal providentFund))
            {
                decimal providentLimit = Math.Min(income * 0.15M, 500000);
                totalRelief += Math.Min(providentFund, providentLimit);
            }

            if (decimal.TryParse(nationalSavingsEntry.Text, out decimal nationalSavings))
                totalRelief += Math.Min(nationalSavings, 30000);

            if (decimal.TryParse(rmfFundEntry.Text, out decimal rmfFund))
            {
                decimal rmfLimit = Math.Min(income * 0.30M, 500000);
                totalRelief += Math.Min(rmfFund, rmfLimit);
            }

            if (decimal.TryParse(ssfFundEntry.Text, out decimal ssfFund))
            {
                decimal ssfLimit = Math.Min(income * 0.30M, 200000);
                totalRelief += Math.Min(ssfFund, ssfLimit);
            }

            if (decimal.TryParse(socialSecurityEntry.Text, out decimal socialSecurity))
                totalRelief += Math.Min(socialSecurity, 9000);

            if (decimal.TryParse(housingLoanInterestEntry.Text, out decimal housingLoanInterest))
                totalRelief += Math.Min(housingLoanInterest, 100000);

            // ลดหย่อนกลุ่มบริจาค
            if (decimal.TryParse(charityEducationEntry.Text, out decimal charityEducation))
            {
                decimal charityEducationDeduction = Math.Min(charityEducation * 2, income * 0.10M);
                totalRelief += charityEducationDeduction;
            }

            if (decimal.TryParse(charityGeneralEntry.Text, out decimal charityGeneral))
            {
                decimal charityGeneralLimit = income * 0.10M;
                totalRelief += Math.Min(charityGeneral, charityGeneralLimit);
            }

            if (decimal.TryParse(politicalDonationEntry.Text, out decimal politicalDonation))
                totalRelief += Math.Min(politicalDonation, 10000);

            // คำนวณรายได้สุทธิ (รายได้ - ลดหย่อน)
            decimal netIncome = income - totalRelief;

            // ตรวจสอบว่ารายได้สุทธิต้องเสียภาษีหรือไม่
            if (netIncome <= 150000)
            {
                resultLabel.Text = $"รายได้สุทธิ: {netIncome:N2} บาท\nคุณไม่ต้องเสียภาษี";
                return;
            }

            // คำนวณภาษีที่ต้องจ่าย
            decimal taxToPay = CalculateTax(netIncome);

            // แสดงผลลัพธ์
            resultLabel.Text = $"ปีภาษีที่เลือก: {selectedTaxYear}\n" +
                               $"รายได้สุทธิ: {netIncome:N2} บาท\n" +
                               $"ยอดลดหย่อนรวม: {totalRelief:N2} บาท\n" +
                               $"ภาษีที่ต้องจ่าย: {taxToPay:N2} บาท";
            this.netIncome = netIncome;
            this.totalRelief = totalRelief;
            this.taxToPay = taxToPay;
            this.selectedTaxYear = selectedTaxYear;
        }

        private decimal CalculateTax(decimal netIncome)
        {
            decimal tax = 0;

            if (netIncome > 5000000)
            {
                tax += (netIncome - 5000000) * 0.35M;
                netIncome = 5000000;
            }
            if (netIncome > 2000000)
            {
                tax += (netIncome - 2000000) * 0.30M;
                netIncome = 2000000;
            }
            if (netIncome > 1000000)
            {
                tax += (netIncome - 1000000) * 0.25M;
                netIncome = 1000000;
            }
            if (netIncome > 750000)
            {
                tax += (netIncome - 750000) * 0.20M;
                netIncome = 750000;
            }
            if (netIncome > 500000)
            {
                tax += (netIncome - 500000) * 0.15M;
                netIncome = 500000;
            }
            if (netIncome > 300000)
            {
                tax += (netIncome - 300000) * 0.10M;
                netIncome = 300000;
            }
            if (netIncome > 150000)
            {
                tax += (netIncome - 150000) * 0.05M;
            }

            return tax;
        }

        private async void SaveButtonClick(object sender, EventArgs e)
        {
            try
            {
                // สร้างโมเดลข้อมูลการคำนวณ
                CalculateModel calculationResult = new CalculateModel();
                {
                    
                    calculationResult.Exempt = this.totalRelief;
                    calculationResult.Income = this.netIncome;
                    calculationResult.TaxAmount = this.taxToPay;
                    calculationResult.TaxYear = this.selectedTaxYear;

                };

                // เรียกใช้บริการเพื่อบันทึกข้อมูลลง Firestore
                FirestoreService fs = new FirestoreService();
                await fs.InsertCalculation(calculationResult);

                // แสดง Popup ว่าบันทึกสำเร็จ
                await DisplayAlert("สำเร็จ", "ข้อมูลถูกบันทึกเรียบร้อยแล้ว", "ตกลง");

                // นำทางกลับไปยังหน้า Home
                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
