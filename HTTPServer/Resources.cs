using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class Resources
{
	private List<Resources> resources = [];

	public List<Resources> Get() => resources;
}

internal record Resource(string name);

