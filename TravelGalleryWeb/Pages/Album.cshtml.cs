using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelGalleryWeb.Models;
using TravelGalleryWeb.Data;

namespace TravelGalleryWeb.Pages
{
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 900)]
    public class AlbumModel : PageModel
    {
        private ApplicationContext _context;
        private readonly IOptions<Constants> _config;
        
        [BindProperty(SupportsGet = true)] public int Id { get; set; }
        [BindProperty(SupportsGet = true)] public int Year { get; set; }
        [BindProperty(SupportsGet = true)] public PhotoTag Tag { get; set; } = PhotoTag.All;
        public string Name;

        public List<Photo> PhotoCollection { get; set; }
        
        public List<int> Years { get; set; }

        public AlbumModel(ApplicationContext context, IOptions<Constants> config)
        {
            _context = context;
            _config = config;
        }
        
        public async Task OnGetAsync()
        {
            Name = _context.Albums.AsNoTracking().FirstOrDefaultAsync(a => a.Id == Id).Result.Name;
            
            Years = await (from p in _context.Photos.AsNoTracking() 
                    where p.AlbumId == Id 
                    select p.Year)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync();

            //Without list length checking All() returns true when the list is empty.
            if (Years.Any() && Years.All(y => y != Year))
            {
                Year = Years.Max();
            }
            
            if (Tag != PhotoTag.All)
            {
                PhotoCollection = await _context.Photos.AsNoTracking()
                    .Where(p => p.AlbumId == Id)
                    .Where(p => p.Year == Year)
                    .Where(p => p.Tag == Tag)
                    .ToListAsync();
            }
            
            else PhotoCollection = await _context.Photos.AsNoTracking()
                .Where(p => p.AlbumId == Id)
                .Where(p => p.Year == Year)
                .ToListAsync();
        }

        public string FindPreview(string path)
        {
            return path.Replace("upload/",
                "upload/w_" + _config.Value.PreviewWidth + "/");
        }
    }
}