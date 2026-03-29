using System;

namespace LabWork1
{
    /// <summary>
    /// Набор подпрограмм для обработки экспериментальных данных (задание к лаб. работам).
    /// </summary>
    public static class StatisticalFunctions
    {
        // ─────────────────────────────────────────────────────────────
        // 1. Математическое ожидание и дисперсия
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Вычисляет оценку математического ожидания M и дисперсии D массива X.
        /// M = (1/N) * ΣX(i)
        /// D = (1/N) * Σ(X(i) - M)²
        /// </summary>
        public static (double M, double D) MeanAndVariance(double[] x)
        {
            if (x == null || x.Length == 0)
                throw new ArgumentException("Массив не должен быть пустым.");

            int n = x.Length;
            double M = 0.0;
            for (int i = 0; i < n; i++) M += x[i];
            M /= n;

            double D = 0.0;
            for (int i = 0; i < n; i++) D += (x[i] - M) * (x[i] - M);
            D /= n;

            return (M, D);
        }

        /// <summary>
        /// Выборочная дисперсия с делителем (N-1) — используется в лаб. работе №1.
        /// </summary>
        public static (double M, double S2) SampleMeanAndVariance(double[] x)
        {
            if (x == null || x.Length < 2)
                throw new ArgumentException("Нужно минимум 2 элемента.");

            int n = x.Length;
            double M = 0.0;
            for (int i = 0; i < n; i++) M += x[i];
            M /= n;

            double S2 = 0.0;
            for (int i = 0; i < n; i++) S2 += (x[i] - M) * (x[i] - M);
            S2 /= (n - 1);

            return (M, S2);
        }

        // ─────────────────────────────────────────────────────────────
        // 2. Плотность вероятности нормального распределения
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// f(X) = 1 / (σ * √(2π)) * exp(-(X-M)² / (2σ²))
        /// </summary>
        /// <param name="x">Значение случайной величины</param>
        /// <param name="M">Математическое ожидание</param>
        /// <param name="sigma2">Дисперсия σ²</param>
        public static double GaussianPDF(double x, double M, double sigma2)
        {
            if (sigma2 <= 0)
                throw new ArgumentException("Дисперсия должна быть положительной.");

            double sigma = Math.Sqrt(sigma2);
            double exponent = -(x - M) * (x - M) / (2.0 * sigma2);
            return Math.Exp(exponent) / (sigma * Math.Sqrt(2.0 * Math.PI));
        }

        // ─────────────────────────────────────────────────────────────
        // 3. Функция распределения гауссовской СВ
        // ─────────────────────────────────────────────────────────────

        // Коэффициенты полинома
        private static readonly double[] D_CDF = {
            4986.7347e-5,   // d1
            2114.1006e-5,   // d2
            327.76263e-5,   // d3
            38.0036e-6,     // d4
            48.8906e-6,     // d5
            53.83e-7        // d6
        };

        /// <summary>
        /// F(X) = 1 - (1/2) * (1 + d1*X + d2*X² + ... + d6*X^6)^(-16)
        /// </summary>
        public static double GaussianCDF(double x)
        {
            double poly = 1.0;
            double xPow = x;
            for (int i = 0; i < 6; i++)
            {
                poly += D_CDF[i] * xPow;
                xPow *= x;
            }
            return 1.0 - 0.5 * Math.Pow(poly, -16.0);
        }

        // ─────────────────────────────────────────────────────────────
        // 4. Квантиль гауссовского распределения
        // ─────────────────────────────────────────────────────────────

        // Коэффициенты аппроксимации
        private const double C0 = 2.515517;
        private const double C1 = 0.8028538;
        private const double C2 = 0.01032;
        private const double D1 = 1.432788;
        private const double D2 = 0.189269;
        private const double D3 = 0.001308;

        /// <summary>
        /// Квантиль гауссовского распределения порядка q.
        /// λq = t - (c0 + c1*t + c2*t²) / (1 + d1*t + d2*t² + d3*t³)
        /// где t = √(ln(α⁻²)), α = 1 - q
        /// </summary>
        public static double GaussianQuantile(double q)
        {
            if (q <= 0.0 || q >= 1.0)
                throw new ArgumentException("Вероятность q должна быть в (0, 1).");

            double alpha = 1.0 - q;
            double t = Math.Sqrt(Math.Log(1.0 / (alpha * alpha)));

            double numerator   = C0 + C1 * t + C2 * t * t;
            double denominator = 1.0 + D1 * t + D2 * t * t + D3 * t * t * t;

            return t - numerator / denominator;
        }

        // ─────────────────────────────────────────────────────────────
        // 5. Квантиль t-распределения Стьюдента
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// t(v, q) = λq + q1/v + q2/v² + q3/v³ + q4/v⁴
        /// </summary>
        /// <param name="v">Число степеней свободы</param>
        /// <param name="q">Вероятность (q = 1 - α/2 для двустороннего критерия)</param>
        public static double StudentQuantile(int v, double q)
        {
            if (v <= 0)
                throw new ArgumentException("Число степеней свободы должно быть положительным.");

            double lq = GaussianQuantile(q);
            double lq2 = lq * lq;

            double q1 = (lq2 + 1.0) * lq / 4.0;
            double q2 = ((5.0 * lq2 + 16.0) * lq2 + 3.0) * lq / 96.0;
            double q3 = (((3.0 * lq2 + 19.0) * lq2 + 17.0) * lq2 - 15.0) * lq / 384.0;
            double q4 = ((((79.0 * lq2 + 776.0) * lq2 + 1482.0) * lq2 - 1920.0) * lq2 - 945.0) * lq / 92160.0;

            double vd = (double)v;
            return lq + q1 / vd + q2 / (vd * vd) + q3 / (vd * vd * vd) + q4 / (vd * vd * vd * vd);
        }

        // ─────────────────────────────────────────────────────────────
        // 6. Квантиль хи-квадрат распределения
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// χ²(v, q) = v * (1 - 2/(9v) + λq * √(2/(9v)))³
        /// </summary>
        public static double ChiSquareQuantile(int v, double q)
        {
            if (v <= 0)
                throw new ArgumentException("Число степеней свободы должно быть положительным.");

            double lq = GaussianQuantile(q);
            double term = 1.0 - 2.0 / (9.0 * v) + lq * Math.Sqrt(2.0 / (9.0 * v));
            return v * term * term * term;
        }

        // ─────────────────────────────────────────────────────────────
        // 7. Квантиль F-распределения (распределения Фишера)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// F(v1, v2, q) = exp(2w)
        /// H = 2*(v1-1)*(v2-1)/(v1+v2-2)
        /// L = (1/6)*(λq² - 3)
        /// w = λq*√(H+L)/H - (1/(v1-1) - 1/(v2-1)) * (L + 5/6 - 2/(3H))
        /// </summary>
        public static double FisherQuantile(int v1, int v2, double q)
        {
            if (v1 <= 1 || v2 <= 1)
                throw new ArgumentException("Числа степеней свободы должны быть больше 1.");

            double lq = GaussianQuantile(q);
            double H = 2.0 * (v1 - 1.0) * (v2 - 1.0) / (v1 + v2 - 2.0);
            double L = (lq * lq - 3.0) / 6.0;

            double w = lq * Math.Sqrt(H + L) / H
                       - (1.0 / (v1 - 1.0) - 1.0 / (v2 - 1.0)) * (L + 5.0 / 6.0 - 2.0 / (3.0 * H));

            return Math.Exp(2.0 * w);
        }
    }
}
