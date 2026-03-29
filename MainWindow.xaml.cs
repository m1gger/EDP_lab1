using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LabWork1
{
    public partial class MainWindow : Window
    {
        // ─── Данные 40 измерений (лаб. работа №1) ─────────────────────
        private static readonly double[] Measurements = {
            528.2, 542.8, 531.2, 597.5, 555.9, 553.2, 539.2, 548.2, 523.5, 561.0,  // 1-10
            546.8, 545.8, 538.4, 541.7, 547.6, 541.9, 551.0, 540.0, 569.8, 529.3,  // 11-20
            524.2, 558.5, 544.0, 545.4, 539.6, 525.5, 592.2, 536.8, 519.9, 505.1,  // 21-30
            536.0, 584.5, 540.3, 544.5, 535.0, 551.3, 558.3, 525.5, 554.7, 542.1   // 31-40
        };

        public MainWindow()
        {
            InitializeComponent();
            LoadMeasurements(outlierIndex: -1);
        }

        // ═══════════════════════════════════════════════════════════════
        //   Вспомогательные методы
        // ═══════════════════════════════════════════════════════════════

        private static double[] ParseArray(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new FormatException("Введите числа.");

            var tokens = input.Split(new[] { ' ', ',', ';', '\t' },
                                     StringSplitOptions.RemoveEmptyEntries);
            return tokens.Select(t =>
                double.Parse(t.Replace(',', '.'), CultureInfo.InvariantCulture)).ToArray();
        }

        private static double ParseDouble(string text)
        {
            if (double.TryParse(text.Trim().Replace(',', '.'),
                                NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                return v;
            throw new FormatException($"Не удаётся распознать число: \"{text}\"");
        }

        private static int ParseInt(string text)
        {
            if (int.TryParse(text.Trim(), out int v)) return v;
            throw new FormatException($"Не удаётся распознать целое число: \"{text}\"");
        }

        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);

        // ═══════════════════════════════════════════════════════════════
        //   Вкладка 1 — обработчики кнопок
        // ═══════════════════════════════════════════════════════════════

        private void CalcMeanVariance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = ParseArray(TbArray1.Text);
                var (M, D) = StatisticalFunctions.MeanAndVariance(x);
                TbResult1.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult1.Text = $"N = {x.Length}\nM  = {M:F6}\nD  = {D:F6}\nσ  = {Math.Sqrt(D):F6}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult1.Text = ""; }
        }

        private void CalcPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x      = ParseDouble(TbPdf_X.Text);
                double M      = ParseDouble(TbPdf_M.Text);
                double sigma2 = ParseDouble(TbPdf_Sigma2.Text);
                double f      = StatisticalFunctions.GaussianPDF(x, M, sigma2);
                TbResult2.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult2.Text = $"f(X = {x}) = {f:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult2.Text = ""; }
        }

        private void CalcCDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = ParseDouble(TbCdf_X.Text);
                double F = StatisticalFunctions.GaussianCDF(x);
                TbResult3.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult3.Text = $"F(X = {x}) = {F:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult3.Text = ""; }
        }

        private void CalcGaussQuantile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double q  = ParseDouble(TbGQ_q.Text);
                double lq = StatisticalFunctions.GaussianQuantile(q);
                TbResult4.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult4.Text = $"λ_q (q = {q}) = {lq:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult4.Text = ""; }
        }

        private void CalcStudentQuantile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int    v = ParseInt(TbSt_v.Text);
                double q = ParseDouble(TbSt_q.Text);
                double t = StatisticalFunctions.StudentQuantile(v, q);
                TbResult5.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult5.Text = $"t(v = {v}, q = {q}) = {t:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult5.Text = ""; }
        }

        private void CalcChiSquareQuantile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int    v = ParseInt(TbChi_v.Text);
                double q = ParseDouble(TbChi_q.Text);
                double c = StatisticalFunctions.ChiSquareQuantile(v, q);
                TbResult6.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult6.Text = $"χ²(v = {v}, q = {q}) = {c:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult6.Text = ""; }
        }

        private void CalcFisherQuantile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int    v1 = ParseInt(TbFish_v1.Text);
                int    v2 = ParseInt(TbFish_v2.Text);
                double q  = ParseDouble(TbFish_q.Text);
                double F  = StatisticalFunctions.FisherQuantile(v1, v2, q);
                TbResult7.Foreground = new SolidColorBrush(Color.FromRgb(0x1A, 0x5C, 0x2A));
                TbResult7.Text = $"F(v1 = {v1}, v2 = {v2}, q = {q}) = {F:G10}";
            }
            catch (Exception ex) { ShowError(ex.Message); TbResult7.Text = ""; }
        }

        // ═══════════════════════════════════════════════════════════════
        //   Вкладка 2 — Лаб. работа №1
        // ═══════════════════════════════════════════════════════════════

        private void LoadMeasurements(int outlierIndex)
        {
            var items = new List<MeasurementRow>();
            for (int i = 0; i < Measurements.Length; i++)
            {
                bool isCandidate = (i == outlierIndex);
                items.Add(new MeasurementRow
                {
                    Index              = i + 1,
                    Value              = Measurements[i],
                    IsOutlierCandidate = isCandidate,
                    HighlightColor     = isCandidate
                                        ? new SolidColorBrush(Colors.Red)
                                        : new SolidColorBrush(Colors.Transparent),
                    HighlightTip       = isCandidate ? "Кандидат на промах" : ""
                });
            }
            DgMeasurements.ItemsSource = items;
        }

        private void RunOutlierAnalysis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double alpha = ParseDouble(TbAlpha.Text);
                if (alpha <= 0 || alpha >= 1)
                    throw new ArgumentException("Уровень значимости α должен быть в (0, 1).");

                int n = Measurements.Length;

                // ── Шаг 1: среднее и выборочная дисперсия (все 40) ──────
                var (mean, s2) = StatisticalFunctions.SampleMeanAndVariance(Measurements);
                double s = Math.Sqrt(s2);

                TbStatMean.Text = mean.ToString("F4");
                TbStatVar.Text  = s2.ToString("F4");
                TbStatStd.Text  = s.ToString("F4");

                // ── Шаг 2: найти кандидата на промах (макс. |X - X̄|) ──
                int    xmaxIdx   = 0;
                double maxDev    = 0;
                for (int i = 0; i < n; i++)
                {
                    double dev = Math.Abs(Measurements[i] - mean);
                    if (dev > maxDev) { maxDev = dev; xmaxIdx = i; }
                }
                double xmax = Measurements[xmaxIdx];
                TbStatXmax.Text = $"X[{xmaxIdx + 1}] = {xmax:F1}  (отклонение {maxDev:F2})";
                LoadMeasurements(xmaxIdx);

                // ── Метод I ──────────────────────────────────────────────
                // u = (Xmax - X̄) / S  (S — СКО по всей выборке)
                double u     = (xmax - mean) / s;
                int    v     = n - 1;
                double q_t   = 1.0 - alpha;         // одностороннее
                double tCrit = StatisticalFunctions.StudentQuantile(v, q_t);

                TbM1_u.Text = u.ToString("F4");
                TbM1_v.Text = v.ToString();
                TbM1_t.Text = $"t({v}, {q_t:F4}) = {tCrit:F4}";

                bool isOutlier1 = u > tCrit;
                TbM1_Conclusion.Text      = isOutlier1
                    ? $"u = {u:F4} > t = {tCrit:F4}  →  X[{xmaxIdx+1}] ЯВЛЯЕТСЯ промахом"
                    : $"u = {u:F4} ≤ t = {tCrit:F4}  →  X[{xmaxIdx+1}] НЕ является промахом";
                TbM1_Conclusion.Foreground = isOutlier1
                    ? new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0x00))
                    : new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0x00));

                // ── Метод II ─────────────────────────────────────────────
                // Вычислить X̄* и S* без Xmax
                var withoutXmax = Measurements
                    .Where((_, i) => i != xmaxIdx)
                    .ToArray();

                var (meanStar, s2Star) = StatisticalFunctions.SampleMeanAndVariance(withoutXmax);
                double sStar = Math.Sqrt(s2Star);
                double Ki    = (xmax - meanStar) / sStar;

                TbM2_Mean.Text = meanStar.ToString("F4");
                TbM2_Var.Text  = s2Star.ToString("F4");
                TbM2_Std.Text  = sStar.ToString("F4");
                TbM2_Ki.Text   = Ki.ToString("F4");

                bool isOutlier2 = Ki > 4.0;
                TbM2_Conclusion.Text      = isOutlier2
                    ? $"Ki = {Ki:F4} > 4  →  X[{xmaxIdx+1}] ЯВЛЯЕТСЯ промахом"
                    : $"Ki = {Ki:F4} ≤ 4  →  X[{xmaxIdx+1}] НЕ является промахом";
                TbM2_Conclusion.Foreground = isOutlier2
                    ? new SolidColorBrush(Color.FromRgb(0xC0, 0x00, 0x00))
                    : new SolidColorBrush(Color.FromRgb(0x00, 0x70, 0x00));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    //   Модель строки таблицы измерений
    // ═══════════════════════════════════════════════════════════════════

    public class MeasurementRow
    {
        public int    Index              { get; set; }
        public double Value              { get; set; }
        public bool   IsOutlierCandidate { get; set; }
        public System.Windows.Media.Brush HighlightColor { get; set; } = null!;
        public string HighlightTip       { get; set; } = "";
    }
}
