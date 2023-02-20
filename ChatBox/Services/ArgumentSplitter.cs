

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBox.Services;

public  class ArgumentSplitter
{
	private string[] _args;
	private HashSet<string> _keys;
	private string _delimeter;
	private Dictionary<string,string> _result;
	public ArgumentSplitter(string[] args, string[] keys, string delimeter = "-")
	{
		_args = args;
		_result =  new();
		_keys = keys.ToHashSet();
		_delimeter = delimeter;
		Process();
	}

	public bool TryGetValue(string key, out string value)
	{
		if(!_keys.Contains(key))
		{
			throw new ArgumentException($"Key {key} is not in the list of keys");
		}
		return _result.TryGetValue(key, out value);
	}

	private void Process()
	{
		for (var i = 0; i < _args.Length; i++)
		{
			var arg = _args[i];

			if (!arg.StartsWith(_delimeter))
			{
				continue;
			}

			var key = arg;

			if (!_keys.Contains(key))
			{
				continue;
			}

			if (i + 1 >= _args.Length)
			{
				continue;
			}

			var value = _args[i + 1];
			_result.Add(key, value);
		}
	}


}