<?

include_once ("include/get_q.php");
include_once ("include/rearrange_title.php");

function castcrew($peep_id,$editP) {
    $EC_select = mysql_query("select e.year,l.title,e.ID from EC e INNER JOIN EC_list l ON e.ECID=l.ID where e.peepID=$peep_id order by e.year DESC");
    $do_EC_check = mysql_num_rows($EC_select);
    if($do_EC_check > 0) {
	echo"<tr align='center'><td colspan=5><b><u>Positions Held</u></b><br><br></td>\n";
	while ($result = mysql_fetch_array($EC_select)) {
	    $ID_to_edit = $result[2];
	    $title = stripslashes($result[1]);
	    $title = rearrange_title($title);
	    echo "<tr><td align=center valign=middle colspan=5>$result[0]&#160; - &#160;$title";
	    if ($editP == 'edit') { editbutton('editEC',$ID_to_edit); }
	}
    }
    $award_select = mysql_query("select a.year,l.name,a.showID,a.ID from awards a INNER JOIN awards_list l ON a.awardID=l.ID where a.peepID=$peep_id order by a.year DESC,l.ID");
    $do_award_check = mysql_num_rows($award_select);
    if($do_award_check > 0) {
	echo"<tr align=center><td colspan=5><br><b><u>Awards</u></b><br><br></td></tr>\n";
	echo"<tr><td align=center colspan=5><table>";
	while ($result = mysql_fetch_array($award_select)) {
	    $ID_to_edit = $result[3];
	    $awardname = stripslashes($result[1]);
	    if ($result[2] > 0) {
		$title_select = mysql_query("SELECT title FROM shows WHERE ID=$result[2]");
		while ($title_list = mysql_fetch_array($title_select)) {
		    $showname = stripslashes($title_list[0]);
		    $showname = rearrange_title($showname);
		}
		echo "	  <tr><td align=center><a href=\"?action=awards_by_year&amp;year=$result[0]\">$result[0]</a></td><td> &#160; - &#160; </td><td align=center>$awardname</td><td> &#160; - &#160; </td><td align=left><a href=\"?action=show_detail&amp;show_id=$result[2]\">$showname</a>";
	    } else {
		echo "	  <tr><td align=center><a href=\"?action=awards_by_year&amp;year=$result[0]\">$result[0]</a></td><td> &#160; - &#160; </td><td align=center>$awardname</td><td>&#160;";
	    }
	    if ($editP == 'edit') { echo"</td><td>";editbutton('editawards',$ID_to_edit); }
	    echo"</td></tr>\n";
	}
	echo"</table>\n";
    }

    $cast_select = mysql_query("select c.role,s.title,s.quarter,s.year,s.ID,c.ID from cast c INNER JOIN shows s ON c.showID=s.ID where c.peepID=$peep_id order by s.year DESC, s.quarter DESC, s.title");
    $do_cast_check = mysql_num_rows($cast_select);
    if($do_cast_check > 0) {
	echo"<tr align='center'><td colspan=2></td><td><br><b><u>Acting Credits</u></b><br><br></td><td colspan=2></td>\n";
	while ($result = mysql_fetch_array($cast_select)) {
	    $title = stripslashes($result[1]);
	    $title = rearrange_title($title);
	    $quarter = get_q($result[2]);
	    $ID_to_edit = $result[5];
	    echo <<<EOT65
		<tr align=center><td align=right>$quarter $result[3]</td>
		<td>&#160; - &#160;</td>
		<td><a href="$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$result[4]">$title</a></td>
		<td>&#160; - &#160;</td>
		<td align=left>$result[0]
EOT65;
	    if ($editP == 'edit') { editbutton('editcast',$ID_to_edit); }
	    echo "</td>";
	}
    }

    $crew_select = mysql_query("select j.job,s.title,s.quarter,s.year,s.ID,c.ID,j.URL from crew c JOIN shows s ON c.showID=s.ID JOIN jobs j ON j.ID=c.jobID where peepID=$peep_id order by s.year DESC, s.quarter DESC, s.title, j.priority");
    $do_crew_check = mysql_num_rows($crew_select);
    if($do_crew_check > 0) {
	echo"<tr align='center'><td colspan=2></td><td align=center><br><br><b><u>Production Credits</u></b><br><br></td><td colspan=2></td>\n";
	while ($result = mysql_fetch_array($crew_select)) {
	    $ID_to_edit = $result[5];
	    $title = stripslashes($result[1]);
	    $title = rearrange_title($title);
	    $quarter = get_q($result[2]);
	    echo <<< EOT66
		<tr align=center valign=middle><td align=right>$quarter $result[3]</td>
		<td align=center valign=middle>-</td>
		<td align=center valign=middle><a href="$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$result[4]">$title</a></td>
		<td align=center valign=middle>-</td>
		<td align=left valign=middle>
EOT66;
	    if (strlen($result[6]) > 3) { 
		echo "<a target='new' href=\"$result[6]\">$result[0]</a>";
	    } else {
		echo "$result[0]";
	    }
	    if ($editP == 'edit') { editbutton('editcrew',$ID_to_edit,$show_id); }
	    echo "</td>";
	}
    }
    echo "</table></td>\n";
    echo"</tr></table>\n";
}

?>
