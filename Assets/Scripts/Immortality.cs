using UnityEngine;
using System.Collections;

public class Immortality : MonoBehaviour {

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}
}