using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance;

	void Awake()
	{
		Instance = this;
	}


}
