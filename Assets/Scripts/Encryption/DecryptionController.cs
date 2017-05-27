using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class DecryptionController : MonoBehaviour {

	private InputField 	_userName;
	private InputField 	_fieldName;
	private InputField 	_encryption;
	private InputField 	_decryption;

	private Button 		_decryptButton;

	private string[][] 	_preKeyMap;
	private string[][] 	_postKeyMap;
	private string[]   	_preCharMap;
	private string[]   	_postCharMap;


	void Start () {
		_userName = GameObject.Find("User Name").GetComponent<InputField>();
		_fieldName = GameObject.Find("Field Name").GetComponent<InputField>();
		_encryption = GameObject.Find("Encrypted").GetComponent<InputField>();
		_decryption = GameObject.Find("Decrypted").GetComponent<InputField>();

		_decryptButton = GameObject.Find("Decrypt").GetComponent<Button>();
		_decryptButton.onClick.AddListener(() => {PreDecryptValue();});

		CreateMaps();
	}

	private void PreDecryptValue() {
		if(_encryption.text != "") {
			if(_userName.text.Length > 4) {
				if(_fieldName.text.Length > 4) {
					DecryptValue();
				}else{
					Debug.Log("fieldname must be at least 5 chars");
				}
			}else{
				Debug.Log("username must be at least 5 chars");
			}
		}else{
			Debug.Log("nothing to decrypt");
		}
	}

	private void DecryptValue() {
		int[] Key = CreateKey();
		_postCharMap = ModifyCharMap(Key);
		_postKeyMap = ModifyKeyMap(Key);

		string tempseed = UnConvertSeed();
		int SeedIndex = Int32.Parse(tempseed.Substring(0, 1));
		int SeedValue = Int32.Parse(tempseed.Substring(1, 1));
		string seedless = _encryption.text.Substring(0, 1) + _encryption.text.Substring(3, _encryption.text.Length - 3);
		if(SeedIndex + SeedValue <= seedless.Length) {
			string strlock1 = seedless.Substring(0, SeedIndex) + seedless.Substring(SeedIndex + SeedValue, seedless.Length - (SeedIndex + SeedValue));
			string convval = seedless.Substring(SeedIndex, SeedValue);

			string val = UnConvertValue(convval);
			string strlock2 = CheckUpdatedValue(val);

			_decryption.text = (strlock1 == strlock2)? val : "Stop cheating";
		}else{
			_decryption.text = "Stop cheating";
		}
	}

	private int[] CreateKey() {
		string user = _userName.text;
		string field = _fieldName.text;
		string strkey =
		field.Substring(0, 1) +
		user.Substring(user.Length-1,1) +
		field.Substring(2, 1) +
		user.Substring(user.Length-3,1) +
		field.Substring(4, 1) +
		user.Substring(user.Length-5,1) +
		field.Substring(4, 1) +
		user.Substring(user.Length-3,1) +
		field.Substring(2, 1) +
		user.Substring(user.Length-1,1) +
		field.Substring(0, 1);

		int[] key = new int[_preKeyMap.Length];
		for(int x = 0; x < strkey.Length; x++) {
			float temp = 0f;
			byte[] bytes = Console.InputEncoding.GetBytes(strkey.Substring(x, 1));
			foreach(byte b in bytes){
				temp = (float)Int32.Parse(b.ToString());
			}
			int temp2 = Int32.Parse(((temp/Mathf.PI - (float)Math.Truncate(temp/Mathf.PI))).ToString().Substring(2, 1));
			key[x] = temp2;
		}
		return key;
	}

	private string[][] ModifyKeyMap(int[] key) {
		string[][] modKeyMap = new string[_preKeyMap.Length][];
		for(int x = 0; x < _preKeyMap.Length; x++) {
			modKeyMap[x] = RotateRight(_preKeyMap[x],key[x]);
		}
		return modKeyMap;
	}

	private string[] ModifyCharMap(int[] key) {
		string[] modCharMap = new string[_preCharMap.Length];
		int num = key.Sum();
		modCharMap = RotateRight(_preCharMap,num);
		return modCharMap;
	}

	private string GetCharFromMap(int value,int index) {
		return (value + index < _postCharMap.Length)? _postCharMap[value + index] : _postCharMap[value + index - _postCharMap.Length];
	}
	private string GetIndexFromChar(string value,int index) {
		bool single = false;
		if(index == -1) {
			single = true;
			index = 0;
		}
		string ind = (Array.IndexOf(_postCharMap, value) - index < 0)? (_postCharMap.Length + Array.IndexOf(_postCharMap, value) - index).ToString() : (Array.IndexOf(_postCharMap, value) - index).ToString();
		return (single)? ind.Substring(0, 1) : (ind.Length == 1)? "0" : ind.Substring(0, 1);
	}

	private string CheckUpdatedValue(string value){
		string fullval = null;
		string tempval = value;
		while(tempval.Length < 9) {
			tempval = "0" + tempval;
		}
		for(int x = 0; x < tempval.Length; x++) {
			int val = Int32.Parse(tempval.Substring(x,1));
			string newval = null;
			newval = (x == 0)? _postKeyMap[x][val] : _postKeyMap[x + 2][val];
			fullval = fullval + newval;
		}
		return fullval;
	}

	private string UnConvertSeed() {
		string seed = GetIndexFromChar(_encryption.text.Substring(1, 1),0) + GetIndexFromChar(_encryption.text.Substring(2, 1), 1);
		return seed;
	}

	private string UnConvertValue(string convval) {
		string val = null;
		if(convval.Length == 1) {
			val = GetIndexFromChar(convval, -1);
		}else{
			for(int x = 0; x < convval.Length; x++) {
				val += GetIndexFromChar(convval.Substring(x, 1), x).Substring(0, 1);
			}
		}
		return val;
	}

	private string[] RotateRight(string[] _arr, int num) {
		string[] temp = _arr;
		for(int x = 0; x < temp.Length - num; x++) {
            temp = RotateLeftOne(temp);
        }
        return temp;
	}

	private string[] RotateLeftOne(string[] _arr) {
		string[] temp = _arr.Skip(1).Concat(_arr.Take(1)).ToArray();
		return temp;
	}

	private void CreateMaps() {
		_preKeyMap = new string[11][];
		_preKeyMap[0] = new string[] {"a","j","s","1","0","^",";","<","G","P"};
		_preKeyMap[1] = new string[] {"1","2","3","4","5","6","7","8","9","0"};
		_preKeyMap[2] = new string[] {"1","2","3","4","5","6","7","8","9","0"};
		_preKeyMap[3] = new string[] {"b","k","t","2","-","&","'",">","H","Q"};
		_preKeyMap[4] = new string[] {"c","l","u","3","=","*",",","?","I","R"};
		_preKeyMap[5] = new string[] {"d","m","v","4","~","(",".","A","J","S"};
		_preKeyMap[6] = new string[] {"e","n","w","5","!",")","/","B","K","T"};
		_preKeyMap[7] = new string[] {"f","o","x","6","@","_","{","C","L","U"};
		_preKeyMap[8] = new string[] {"g","p","y","7","#","+","}","D","M","V"};
		_preKeyMap[9] = new string[] {"h","q","z","8","$","[","|","E","N","W"};
		_preKeyMap[10] = new string[] {"i","r","`","9","%","]",":","F","O","X"};

		_postKeyMap = new string[11][];
		for(int x = 0; x < _postKeyMap.Length; x++) {
			_postKeyMap[x] = new string[10];
		}

		_preCharMap = new string[]{
			"a","b","c","d","e","f","g","h","i","j",
			"k","l","m","n","o","p","q","r","s","t",
			"u","v","w","x","y","z","`","1","2","3",
			"4","5","6","7","8","9","0","-","=","~",
			"!","@","#","$","%","^","&","*","(",")",
			"_","+","[","]",";","'",",",".","/","{",
			"}","|",":","<",">","?","A","B","C","D",
			"E","F","G","H","I","J","K","L","M","N",
			"O","P","Q","R","S","T","U","V","W","X",
			"Y","Z","å","∫","ç","∂","´","ƒ","©","˙",
			};

		_postCharMap = new string[100];
	}
}
