using AutoMapper;
using T4;
using OptimizelyGraph.Models.Pages;

namespace OptimizelyGraph.ContentMigration
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticlePage>()
                 .ForMember(x => x.Name, y => y.MapFrom(from => from.Title.Value))
                 .ForMember(x => x.TeaserText, y => y.MapFrom(from => from.Summary.Value))
                 .ForMember(x => x.MainBody, y => y.MapFrom(from => from.Body.Value));

        }
    }
}
