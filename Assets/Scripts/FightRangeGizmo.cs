using UnityEngine;
using System.Collections;

public class FightRangeGizmo : MonoBehaviour {

	public float[] ranges;
	
	private Color[] color = new Color[] {
		Color.red, Color.magenta, Color.yellow, Color.green,
		Color.blue, Color.cyan, Color.white
	};

	void Start() {}

	void OnDrawGizmosSelected() {
		if(this.isActiveAndEnabled) {
			foreach(float f in ranges) {
				Gizmos.color = color[System.Array.IndexOf(ranges, f) % color.Length];
				Gizmos.DrawWireSphere(transform.position, f);
			}
		}
	}
}
