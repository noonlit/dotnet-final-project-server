using AutoMapper;
using FinalProject.Models;
using FinalProject.ViewModels;
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
		}
	}
}
