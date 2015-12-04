<?
	$type = ($_GET["type"] == "crew" ? "crew" : "cast");
	include_once ("include/rearrange_title.php");
	include_once("include/get_pic.php");

	function searchNames($name)
	{
		$searchstrings = explode(" ", $name);
		for ($i = 0; $i < sizeof($searchstrings); $i++) {
			if ($searchstrings[$i] == "") continue;
		
			if ($i) {
				$peep_query .= " and ";
			}
			$searchstrings[$i] = mysql_real_escape_string($searchstrings[$i]);
			$peep_query .= "(lname like '%" . $searchstrings[$i] . "%' or fname like '%" . $searchstrings[$i] . "%' or nickname like '%" . $searchstrings[$i] . "%')";
		}
		return $peep_query;
	}

	function arraytoString($array)
	{
		for ($i = 0; $i < sizeof($array); $i++)
			$retVal .= ($retVal ? ", " : "") . $array[$i];

		if ($retVal)
			return $retVal;

		return "-1";
	}

	function countDegrees($curPeep, $lookingFor)
	{
		global $type;

		if ($curPeep == $lookingFor)
		{
			//echo "<center><b>Idiot...</b></center><br>";
			return array($curPeep);
		}

		$showArray = array();
		$peepArray = array();
		$showLookupArray = array();
		$peepLookupArray = array();
		$searchedShows = "";
		$searchedPeep = "$curPeep";

		$peepArray[0][] = $curPeep;
		$peepLookupArray[0][] = 0;
		for ($i = 0; $i < 6; $i++)
		{
			$query = mysql_query("select showID, peepID from $type where peepID in (" . arraytoString($peepArray[$i]) . ")" . ($searchedShows ? " and showID not in ($searchedShows)" : ""));

			while ($result = mysql_fetch_array($query))
			{
				$showArray[$i][] = $result["showID"];
				$showLookupArray[$i][] = $result["peepID"];
			}
			$searchedShows .= ($searchedShows ? ", " : "") . arraytoString($showArray[$i]);

			$query = mysql_query("select peepID, showID from $type where showID in (" . arraytoString($showArray[$i]) . ") and peepID not in ($searchedPeep)");

			while ($result = mysql_fetch_array($query))
			{
				$peepArray[$i+1][] = $result["peepID"];
				$peepLookupArray[$i+1][] = $result["showID"];
			}
			$searchedPeep .= ", " . arraytoString($peepArray[$i+1]);

			if (in_array($lookingFor, $peepArray[$i+1]))
			{
				$retVal[] = $lookingFor;
				$curPeep = $lookingFor;
				while ($i >= 0)
				{
					$found = array_search($curPeep, $peepArray[$i+1]);
					$curShow = $peepLookupArray[$i+1][$found];
					$retVal[] = $curShow;
					$found = array_search($curShow, $showArray[$i]);
					$curPeep = $showLookupArray[$i][$found];
					$retVal[] = $curPeep;
					$i--;
				}
				return $retVal;
			}
		}
		return 0;
	}

	function six_degrees()
	{
		global $type;
		check_number($_GET["peep1"]);
		check_number($_GET["peep2"]);
		if ($_GET["peep1"] && $_GET["peep2"])
		{	
			if ($retVal = countDegrees($_GET["peep1"], $_GET["peep2"]))
			{
				echo "<table align=center border=1><tr>";
				for ($i = sizeof($retVal) - 1; $i >= 0; $i--)
				{
					if ($i % 2 == 0)
					{
						$query = mysql_query("select concat(fname, ' ', lname) from people where ID = " . $retVal[$i]);
						$result = mysql_fetch_row($query);
						$tinypic = get_tinypic('peep',$retVal[$i]);
						echo "<td align=\"center\"><a href=\"" . $_SERVER[PHP_SELF] . "?action=peep_detail&amp;peep_id=" . $retVal[$i] . "\"><img border=\"0\" src=\"" . $tinypic . "\"><br>" . $result[0] . "</a></td>";
					}
					else
					{
						$query = mysql_query("select title from shows where ID = " . $retVal[$i]);
						$result = mysql_fetch_row($query);
						$tinypic = get_tinypic('show',$retVal[$i]);
						$result[0] = rearrange_title($result[0]);
						echo "<td align=\"center\"><a href=\"" . $_SERVER[PHP_SELF] . "?action=show_detail&amp;show_id=" . $retVal[$i] . "\"><img border=\"0\" src=\"" . $tinypic . "\"><br>" . $result[0] . "</a></td>";
					}
	
				}
				echo "</tr></table><br><center>Note. This is only the first path found of this degree. There may be other paths of greater or equal degrees.</center>";
			}
			else
				echo "Too many degrees of separation...";
	
			mysql_close();
		}
		else if ($_POST)
 		{
			$failed = 0;
			echo "<center>";
	
			$query = mysql_query("(select ID, concat(lname, ', ', fname) as title from people where " . searchNames($_POST["name1"]) . ") order by title");
			if (mysql_num_rows($query) == 0)
			{
				$failed++;
				echo "No results found for " . $_POST["name1"] . ".<br>";
			}
			else if (mysql_num_rows($query) == 1)
			{
				$result = mysql_fetch_array($query);
?>
			<form method="get" action="<?=$_SERVER[PHP_SELF]?>">
			<input type="hidden" name="action" value="six_degrees">
			<input type="hidden" name="peep1" value="<?=$result["ID"]?>"><b><?=$result["title"]?></b> and 
<?
			}
			else
			{
				echo "<form method=\"get\" action=\"" . $_SERVER[PHP_SELF] . "\"><input type=\"hidden\" name=\"action\" value=\"six_degrees\"><select name=\"peep1\">";
				while ($result = mysql_fetch_array($query))
					echo "<option value=\"" . $result["ID"] . "\">" . $result["title"] . "</option>";
				echo "</select> and ";
			}
	
			$query = mysql_query("(select ID, concat(lname, ', ', fname) as title from people where " . searchNames($_POST["name2"]) . ") order by title");
			if (mysql_num_rows($query) == 0)
			{
				$failed++;
				echo "No results found for " . $_POST["name2"] . ".";
			}
			else if (mysql_num_rows($query) == 1)
			{
				$result = mysql_fetch_array($query);
?>
		<input type="hidden" name="peep2" value="<?=$result["ID"]?>"><b><?=$result["title"]?></b>
<?
			}
			else
			{
				echo "<select name=\"peep2\">";
				while ($result = mysql_fetch_array($query))
					echo "<option value=\"" . $result["ID"] . "\">" . $result["title"] . "</option>";
				echo "</select>";
			}
	
			if ($failed == 0)
				echo "<br><select name=\"type\"><option value=\"cast\">Cast</option><option value=\"crew\">Crew</option></select> <input type=\"submit\" value=\"Go\"></form>";
			else if ($failed == 1)
				echo "</form>";
?>
			<form method="post" action="<?=$_SERVER[PHP_SELF]?>?action=six_degrees&amp;type=<?=$type?>">
			<table align=center border=1>
			<tr><td>
			Search Again:<br><br>
			Person 1: <input name="name1" value="<?=$_POST["name1"]?>"><br>
			Person 2: <input name="name2" value="<?=$_POST["name2"]?>"><br>
			<center><input type="submit" value="Search"></center
			</td></tr>
			</table>
			</form>
			</center>
<?
		}
		else
		{
?>
			<form method="post" action="<?=$_SERVER[PHP_SELF]?>?action=six_degrees&amp;type=<?=$type?>">
			<table align=center border=1>
			<tr><td>
			Person 1: <input name="name1"><br>
			Person 2: <input name="name2"><br>
			<center><input type="submit" value="Search"></center>
			</td></tr>
			</table>
			</form>
<?
		}
	}
?>
