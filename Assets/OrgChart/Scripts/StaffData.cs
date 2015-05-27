using UnityEngine;
using System.Collections;

public class StaffData {

	public int baseLevel;
  public int lastLevel;
  public int age;
  public string name;
  public float health = 1.0f;
//  public int tier = 3;
//	public int skillDiff = 0;
//	public StaffData[] children = new StaffData[0];
	public Color shirtsColor;
	public Color tieColor;
	public Color suitsColor;
	public Color hairColor;
	public Color faceColor;

  public Jobs job;
//  public StaffSkill staffSkill;
//	public bool isHired = false;
//	public bool isAssigned = false;
//	public int skillType;
}
public enum Jobs {Research, Develop, Market};

public class StaffSkill{
  public int research;
  public int develop;
  public int market;
}