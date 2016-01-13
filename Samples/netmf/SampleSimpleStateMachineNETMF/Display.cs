using System;
using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace SampleSimpleStateMachineNETMF
{
    static class Display
    {
        private static readonly Bitmap m_screen;
        private static readonly string[] m_lines;
        private static readonly Font m_font;
        private static readonly int m_fontHeight;
        private static readonly int m_screenLines;
        private static readonly char[] m_newLineSeparators;
        private static int m_linesIndex;
        private static int m_linesCount;

        static Display()
        {
            int width, height, bitsPerPixel, orientationDeg;
            HardwareProvider.HwProvider.GetLCDMetrics(out width, out height, out bitsPerPixel, out orientationDeg);
            m_screen = new Bitmap(width, height);
            m_newLineSeparators = new char[] { '\n' };
            m_font = Resources.GetFont(Resources.FontResources.small);
            m_fontHeight = m_font.Height;
            m_screenLines = height / m_fontHeight;
            m_lines = new string[m_screenLines];
            m_linesCount = 0;

            m_screen.Clear();
            m_screen.Flush();
        }

        public static void WriteLine(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            string[] lines = text.Split(m_newLineSeparators);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                m_lines[m_linesIndex] = (line[line.Length - 1] != '\r') ? line : line.Substring(0, line.Length - 1);
                m_linesIndex = GetLinesIndexModulo(m_linesIndex + 1);
                m_linesCount = System.Math.Min(m_linesCount + 1, m_screenLines);
            }
            m_screen.Clear();
            for (int i = 0; i < m_linesCount; i++)
            {
                int index = GetLinesIndexModulo(m_linesIndex + i + (m_screenLines - m_linesCount));
                m_screen.DrawText(m_lines[index], m_font, Microsoft.SPOT.Presentation.Media.Color.White, 0, i * m_fontHeight);
            }
            m_screen.Flush();
        }

        private static int GetLinesIndexModulo(int index)
        {
            return index < m_screenLines ? index : index - m_screenLines;
        }
    }
}
