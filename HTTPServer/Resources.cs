using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Resources
{
	private List<Resource> resources = [];

	public List<Resource> Get() => resources;

	public void Add(Resource resource) => resources.Add(resource);

	public bool Remove(Resource resource) => resources.Remove(resource);
	public void Remove(int position) => resources.RemoveAt(position);
}

internal record Resource(string Name);

