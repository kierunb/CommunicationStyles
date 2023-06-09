﻿using WCF.Service.Contracts;

namespace WCF.Service.Services;

public class CalculatorService : ICalculatorService
{
    public double Add(double n1, double n2)
    {
        return n1 + n2;
    }

    public double Subtract(double n1, double n2)
    {
        return n1 - n2;
    }

    public double Multiply(double n1, double n2)
    {
        return n1 * n2;
    }

    public double Divide(double n1, double n2)
    {
        return n1 / n2;
    }

    public int AnalyzePayload(PayloadRequest payload)
    {
        return payload.Name?.Length ?? 0;
    }
}