
using GameDevWare.Serialization;

public class Stat  {

	public Stat() { }

	public Stat(string name, int val=1, bool isPrimaryStat = false) {
		this.isPrimaryStat = isPrimaryStat;
		Name = name;
		BaseValue = val;
		BuffValue = 0;

	}

	private Stat(Stat other) {
		isPrimaryStat = other.isPrimaryStat;
		Name = other.Name;
		BaseValue = other.BaseValue;
		BuffValue = other.BuffValue;
	}

	public bool isPrimaryStat { get; set; }

	public string Name { get; set; }

	public int BaseValue { get; set; }

	public int BuffValue { get; set; }

	/// <summary>
	/// Reads the prototype from the specified JObject.
	/// </summary>
	/// <param name="jsonProto">The JProperty containing the prototype.</param>
	//public void ReadJsonPrototype(JProperty jsonProto) {
	//	Type = jsonProto.Name;
	//	JToken innerJson = jsonProto.Value;
	//	Name = PrototypeReader.ReadJson(Name, innerJson["Name"]);
	//}

	public Stat Clone() {
		return new Stat(this);
	}

	public override string ToString() {
		return string.Format("{0}: {1}", Name, BaseValue+ BuffValue);
	}
}
