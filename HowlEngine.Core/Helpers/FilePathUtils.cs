using System;

namespace HowlEngine.Helpers;

public static class FilePathUtils{

    /// <summary>
    /// Returns a substring of a file path.
    /// </summary>
    /// <param name="marker">The sub-string used as the marker to remove all characters before (inclusive).</param>
    /// <param name="path">The full string of the path to strip.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string StripBefore(string marker, string path){

        // Search for the last occurance of 'marker' within 'path'.

        int index = path.LastIndexOf(marker);

        // return a substring of 'path' starting after 'marker'.

        return index >= 0 ? path[(index + marker.Length)..] : throw new InvalidOperationException($"{marker} is not a substring of {path}.");
    }
}