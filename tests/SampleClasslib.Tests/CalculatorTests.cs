// Copyright (C) 2026 Michael Barbeaux. Licensed under the GNU General Public License v3.0 or later. See the LICENSE file for details.

namespace SampleClasslib.Tests;

public class CalculatorTests
{
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(int.MaxValue, 0, int.MaxValue)]
    public void Add_ReturnsSumOfOperands(int left, int right, int expected)
    {
        int result = Calculator.Add(left, right);

        Assert.Equal(expected, result);
    }
}