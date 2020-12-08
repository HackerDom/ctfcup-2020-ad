using System.Collections.Generic;
using QueenOfHearts.CoreLibrary.Models;

namespace QueenOfHearts.VictimsProvider
{
    public interface IVictimIndex
    {
        IEnumerable<string> Names { get; }
        bool TryGetVictim(string name, out Victim victim);
        void Set(Victim victim);
    }
}