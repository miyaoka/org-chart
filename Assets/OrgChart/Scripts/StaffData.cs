using UnityEngine;
using System.Collections;

public class StaffData {

	public int baseSkill;
  public int lastSkill;
  public int age;
//  public int tier = 3;
//	public int skillDiff = 0;
//	public StaffData[] children = new StaffData[0];
	public Color shirtsColor;
	public Color tieColor;
	public Color suitsColor;
	public Color hairColor;
	public Color faceColor;

  public int job;
//	public bool isHired = false;
//	public bool isAssigned = false;
//	public int skillType;
  public enum Jobs {Research, Develop, Market};
}
