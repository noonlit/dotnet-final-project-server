using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;

namespace tests
{
	public class OperationalStoreForTests
	{
		public class OperationalStoreOptionsForTests : IOptions<OperationalStoreOptions>
		{
			public OperationalStoreOptions Value => new OperationalStoreOptions();
		}
	}
}