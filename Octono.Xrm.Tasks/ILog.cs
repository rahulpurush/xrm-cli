﻿namespace Octono.Xrm.Tasks
{
    public interface ILog
    {
        void Write(string message);
        string Prompt(string message);
    }
}