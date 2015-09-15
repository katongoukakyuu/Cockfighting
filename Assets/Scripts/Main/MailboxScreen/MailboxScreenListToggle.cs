using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MailboxScreenListToggle : MonoBehaviour {

	public void ToggleChanged() {
		MailboxManager.Instance.CheckDeleteCount();
	}

}
