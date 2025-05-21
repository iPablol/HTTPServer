using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Resources
{
	private readonly Object resourcesListLock = new();
	private List<Resource> resources = [new("Primero"), new("Segundo")];

	public List<Resource> Get() => resources;

	public void Add(Resource resource) { lock (resourcesListLock) { resources.Add(resource); } }

	public void Edit(int pos, Resource resource) { lock (resourcesListLock) { resources[pos] = resource; } }

	public bool Remove(Resource resource) { lock (resourcesListLock) { return resources.Remove(resource); } }
	public void Remove(int position) { lock (resourcesListLock) { resources.RemoveAt(position); } }
}

internal record Resource(string Name);

