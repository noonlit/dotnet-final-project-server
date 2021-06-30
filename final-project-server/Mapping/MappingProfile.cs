using AutoMapper;
using FinalProject.Models;
using FinalProject.Models.Stats;
using FinalProject.ViewModels;
using FinalProject.ViewModels.Stats;

namespace FinalProject.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Story, StoryViewModel>().ReverseMap();
			CreateMap<Comment, CommentViewModel>().ReverseMap();
			CreateMap<Fragment, FragmentViewModel>().ReverseMap();
			CreateMap<ApplicationUser, AuthorViewModel>().ReverseMap();
			CreateMap<Tag, TagViewModel>().ReverseMap();
			CreateMap<PopularTags, PopularTagsViewModel>().ReverseMap();
		}
	}
}
