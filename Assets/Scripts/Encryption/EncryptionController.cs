using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class EncryptionController : MonoBehaviour {

	private InputField 	_userName;
	private InputField 	_fieldName;
	private InputField 	_value;
	private InputField 	_encryption;

	private Button 		_encryptButton;

	private string[][] 	_preKeyMap;
	private string[][] 	_postKeyMap;
	private string[]   	_preCharMap;
	private string[]   	_postCharMap;


	void Start () {
		_userName = GameObject.Find("User Name").GetComponent<InputField>();
		_fieldName = GameObject.Find("Field Name").GetComponent<InputField>();
		_value = GameObject.Find("Value").GetComponent<InputField>();
		_encryption = GameObject.Find("Encrypted").GetComponent<InputField>();

		_encryptButton = GameObject.Find("Encrypt").GetComponent<Button>();
		_encryptButton.onClick.AddListener(() => {PreEncryptValue();});

		CreateMaps();
	}

	private void PreEncryptValue() {
		_value.text = Int32.Parse(_value.text).ToString();
		if(_value.text != "" && _value.text.Length < 10) {
			if(_userName.text.Length > 4) {
				if(_fieldName.text.Length > 4) {
					EncryptValue();
				}else{
					Debug.Log("fieldname must be at least 5 chars");
				}
			}else{
				Debug.Log("username must be at least 5 chars");
			}
		}else{
			Debug.Log("put a number less than 10 digits in value");
		}
	}

	private void EncryptValue() {
		int[] Key = CreateKey();
		_postCharMap = ModifyCharMap(Key);
		_postKeyMap = ModifyKeyMap(Key);

		string tempval = UpdateValue();

		int SeedIndex = GetSeedIndex();
		int SeedValue = _value.text.Length;
		string convseed = ConvertSeed(SeedIndex,SeedValue);
		string convval = ConvertValue();

		string temp2val = tempval.Substring(0,SeedIndex) + convval + tempval.Substring(SeedIndex, tempval.Length - SeedIndex);
		string finalval = temp2val.Substring(0, 1) + convseed + temp2val.Substring(1,temp2val.Length-1);
		_encryption.text = finalval;
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

	private string UpdateValue(){
		string fullval = null;
		string tempval = _value.text;
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

	private string ConvertSeed(int index, int value) {
		string indexinput = index + value.ToString();
		string valueinput = value + index.ToString();
		string seedindex = GetCharFromMap(Int32.Parse(indexinput), 0);
		string seedvalue = GetCharFromMap(Int32.Parse(valueinput), 1);
		return seedindex + seedvalue;
	}

	private string ConvertValue() {
		string val = _value.text;
		string conv = null;
		if(val.Length == 1) {
			conv = GetCharFromMap(Int32.Parse(val), 0);
		}else{
			for(int x = 0; x < val.Length; x++) {
				conv = (x == val.Length - 1)? conv + GetCharFromMap(Int32.Parse(val.Substring(x, 1) + val.Substring(0, 1)),x) : conv + GetCharFromMap(Int32.Parse(val.Substring(x, 2)),x);
			}
		}
		return conv;
	}

	private int GetSeedIndex() {
		// Can return 3 to 9
		return Mathf.FloorToInt(3f + UnityEngine.Random.value * 6.99f);
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
