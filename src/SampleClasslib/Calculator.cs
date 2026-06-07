// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

namespace SampleClasslib;

/// <summary>
/// Provides basic arithmetic operations.
/// </summary>
public static class Calculator
{
    /// <summary>
    /// Adds two integers.
    /// </summary>
    /// <param name="left">The first operand.</param>
    /// <param name="right">The second operand.</param>
    /// <returns>The sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
    public static int Add(int left, int right) => left + right;
}