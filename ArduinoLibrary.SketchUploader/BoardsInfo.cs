using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ArduinoLibrary.SketchUploader
{
    /// <summary>
    /// loads the ardiono board descriptions from the 
    /// contents of the boards.txt file and makes classes out of it
    /// </summary>
    class BoardsInfo : IEnumerable<Board>
    {
        public BoardsInfo(string boardsText)
        {

            boards = Regex.Split(boardsText, "##############################################################")
                .Where(p => p.Contains(".name="))
                .Select(p => new Board(p))
                .ToList()
                ;
        }

        IList<Board> boards;

        public IEnumerator<Board> GetEnumerator()
        {
            return boards.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
