using System;
using System.IO;
using UnityEngine;

public class CSVLoggerUtil {
    StreamWriter writer;

    string name;
    string directory;
    

    public CSVLoggerUtil(string namebase, string directory, string[] headers)
    {
        this.directory=directory;
        
    }
}