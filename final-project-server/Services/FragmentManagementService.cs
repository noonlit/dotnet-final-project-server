using FinalProject.Data;
using FinalProject.Errors;
using FinalProject.Models;
using FinalProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
	public class FragmentManagementService : IFragmentManagementService
	{
		public ApplicationDbContext _context;
		public FragmentManagementService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> CreateFragment(Fragment fragment)
		{
			_context.Fragments.Add(fragment);
			var serviceResponse = new ServiceResponse<Fragment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = fragment;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>> GetFragmentsForUser(string userId)
		{
			var result = await _context.Fragments.Where(f => f.User.Id == userId).Include(f => f.Story).OrderByDescending(f => f.Story.CreatedAt).ToListAsync();
			var serviceResponse = new ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>> GetFragmentsForStory(int storyId)
		{
			var result = await _context.Fragments.Where(f => f.StoryId == storyId).Include(f => f.Story).OrderByDescending(f => f.Story.CreatedAt).ToListAsync();
			var serviceResponse = new ServiceResponse<List<Fragment>, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> UpdateFragment(Fragment fragment)
		{
			_context.Entry(fragment).State = EntityState.Modified;
			var serviceResponse = new ServiceResponse<Fragment, IEnumerable<EntityManagementError>>();

			try
			{
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = fragment;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public async Task<ServiceResponse<Fragment, IEnumerable<EntityManagementError>>> GetFragment(int id)
		{
			var result = await _context.Fragments.Where(f => f.Id == id).FirstOrDefaultAsync();
			var serviceResponse = new ServiceResponse<Fragment, IEnumerable<EntityManagementError>>();
			serviceResponse.ResponseOk = result;
			return serviceResponse;
		}

		public async Task<ServiceResponse<bool, IEnumerable<EntityManagementError>>> DeleteFragment(int id)
		{
			var serviceResponse = new ServiceResponse<bool, IEnumerable<EntityManagementError>>();

			try
			{
				var favourite = await _context.Fragments.FindAsync(id);
				_context.Fragments.Remove(favourite);
				await _context.SaveChangesAsync();
				serviceResponse.ResponseOk = true;
			}
			catch (Exception e)
			{
				var errors = new List<EntityManagementError>();
				errors.Add(new EntityManagementError { Code = e.GetType().ToString(), Description = e.Message });
			}

			return serviceResponse;
		}

		public bool FragmentExists(int fragmentId)
		{
			return _context.Fragments.Any(e => e.Id == fragmentId);
		}
	}
}
