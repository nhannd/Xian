using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Computes the difference between two strings.  The speed and memory requirements are 
    /// to be O(n2) for this algorithm, so it should not be used on very long strings.
    /// 
    /// Adapted from an algorithm presented here in Javascript:
    /// http://www.csse.monash.edu.au/~lloyd/tildeAlgDS/Dynamic/Edit/
    /// </summary>
    /// <remarks>
    /// The <see cref="AlignedLeft"/> and  <see cref="AlignedRight"/> properties return versions
    /// of the left and right strings that are as closely aligned as possible on a character by
    /// character basis.  '-' characters are inserted into both strings at specific points so as to
    /// produce the closest possible alignment, such that <code>AlignedLeft.Length == AlignedRight.Length</code>. 
    /// The <see cref="DiffMask"/> property is a string of the same length that contains a '|' character
    /// where the aligned strings match and a space where they don't, e.g.
    /// <code>DiffMask[i] = (AlignedLeft[i] == AlignedRight[i]) ? '|' : ' '</code>
    /// </remarks>
    public class StringDiff
    {
        public static StringDiff Compute(string left, string right)
        {
            if (left == right)
            {
                // nop
                return new StringDiff(left, right, new string('|', left.Length));
            }

            string[] result = ComputeDiff(left, right);
            return new StringDiff(result[0], result[2], result[1]);
        }


        private string _alignedLeft;
        private string _alignedRight;
        private string _diffMask;

        public StringDiff(string alignedLeft, string alignedRight, string diffMask)
        {
            _alignedLeft = alignedLeft;
            _alignedRight = alignedRight;
            _diffMask = diffMask;
        }

        public string AlignedLeft
        {
            get { return _alignedLeft; }
        }

        public string AlignedRight
        {
            get { return _alignedRight; }
        }

        public string DiffMask
        {
            get { return _diffMask; }
        }

        private static string[] ComputeDiff(string s1, string s2)
        {
            int[,] m = new int[s1.Length + 1, s2.Length + 1];

            m[0, 0] = 0; // boundary conditions

            for (int j = 1; j <= s2.Length; j++)
                m[0, j] = m[0, j - 1] - 0 + 1; // boundary conditions

            for (int i = 1; i <= s1.Length; i++)                            // outer loop
            {
                m[i, 0] = m[i - 1, 0] - 0 + 1; // boundary conditions

                for (int j = 1; j <= s2.Length; j++)                         // inner loop
                {
                    int diag = m[i - 1, j - 1];
                    if (s1[i - 1] != s2[j - 1]) diag++;

                    m[i, j] = Math.Min(diag,               // match or change
                           Math.Min(m[i - 1, j] - 0 + 1,    // deletion
                                     m[i, j - 1] - 0 + 1)); // insertion
                }//for j
            }//for i

            return traceBack("", "", "", m, s1.Length, s2.Length, s1, s2);
        }

        private static string[] traceBack(string row1, string row2, string row3, int[,] m, int i, int j, string s1, string s2)
        {
            // recover the alignment of s1 and s2
            if (i > 0 && j > 0)
            {
                int diag = m[i - 1, j - 1];
                char diagCh = '|';

                if (s1[i - 1] != s2[j - 1]) { diag++; diagCh = ' '; }

                if (m[i, j] == diag) //LAllison comp sci monash uni au
                    return traceBack(' ' + row1, diagCh + row2, ' ' + row3,
                              m, i - 1, j - 1, s1, s2);    // change or match
                else if (m[i, j] == m[i - 1, j] - 0 + 1) // delete
                    return traceBack(' ' + row1, ' ' + row2, '-' + row3,
                              m, i - 1, j, s1, s2);
                else
                    return traceBack('-' + row1, ' ' + row2, ' ' + row3,
                              m, i, j - 1, s1, s2);      // insertion
            }
            else if (i > 0)
                return traceBack(' ' + row1, ' ' + row2, '-' + row3, m, i - 1, j, s1, s2);
            else if (j > 0)
                return traceBack('-' + row1, ' ' + row2, ' ' + row3, m, i, j - 1, s1, s2);
            else // i==0 and j==0
            {
                return new string[] { row1, row2, row3 };
            }
        }//traceBack
    }
}
