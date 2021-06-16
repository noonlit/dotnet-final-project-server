using FinalProject.Errors;
using FinalProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public interface IFragmentManagementService
	{
		Task<ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>> GetFragmentsForUser(string userId);
		Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> GetFragment(int id);
		Task<ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>> GetFragmentsForStory(int storyId);
		Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> CreateFragment(Fragment fragment);
		Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment);
		Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFragment(int id);
		bool FragmentExists(int fragmentId);
	}
}
