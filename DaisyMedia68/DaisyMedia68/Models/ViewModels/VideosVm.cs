using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DaisyMvc.Models.ViewModels
{
    public class VideosVm
    {

        public ICollection<VideoFile> Videos { get; set; }

    }
}