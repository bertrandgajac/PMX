//using System;
//using System.Collections.Generic;
using System.IO;
//using System.Text;
using System.Threading.Tasks;

namespace Controles
{
    public interface IAccesAuxPhotos
    {
        Task<Stream> DonnerStreamVersPhotoAsync();
    }
}
