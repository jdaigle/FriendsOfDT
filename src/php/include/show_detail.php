<?

include_once ("include/get_fullname.php");
include_once ("include/get_pic.php");
include_once ("include/get_q.php");
include_once ("include/rearrange_title.php");

function show_show_detail($show_id,$editP) {
    $order_select = mysql_query("SELECT ID FROM shows order by year,quarter,title");
    while ($order_result = mysql_fetch_array($order_select)) {
	if (isset($next)) {
	    if ($next == 'next') {
		$next = $order_result[0];
	    }
	}
	if ($order_result[0] == $show_id && !isset($next)) {
	    $next = 'next';
	} elseif (!isset($next)) {
	    $last = $order_result[0];
	}
    }
    if ($last > 0) {
	$prevlink = "<a href=\"$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$last\"><--- Previous Show</a>";
    } else {
	$prevlink = "<--- Previous Show";
    }
    if ($next == 'next') {
	$nextlink = "Next Show --->";
    } else {
	$nextlink = "<a href=\"$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$next\">Next Show ---></a>\n";
    }
    $show_select = mysql_query("select title,quarter,year,pictures,author,funfacts,toaster from shows where ID=$show_id");
    while ($result = mysql_fetch_array($show_select)) {
	$title=stripslashes($result[0]);
	$disptitle=rearrange_title($title);
	$quarter=get_q($result[1]);
	$date="$quarter $result[2]";
	$pictures=$result[3];
	$author=$result[4];
	$funfacts = str_replace("\n", "<br>", $result[5]);
	$toaster = str_replace("\n", "<br>", $result[6]);
	$image_link=get_thumb_link('shows',$show_id);

	echo "<table border=0 align=center width=100%><tr align=center><td align=left valign=top>$prevlink</td>\n";
	echo "<td align=center><center><font size=+4>$disptitle</font><br>";
	if (strlen($author) > 1 ) {
	    echo "<font size=+3>by $author</font><br>";
	}
	echo "<font size=+2><b>$date</b></font></center></td>";
	echo "<td align=right valign=top>$nextlink</td></tr></table><br>\n";
	echo <<< EOT4
	<table align=center border=0 width=100%>
	<tr>
	<td align=center valign=top width='30%'>
	$image_link
	<br>Click poster to see original size<br><br>
	<a href="$_SERVER[PHP_SELF]?action=media_index&amp;IDtype=shows&amp;ID=$show_id">See all media associated with this show</a><br><br>
EOT4;

	if (strlen($pictures) > 1 ) {
	    echo "<br>Click <a href='$pictures' target='_blank'>here</a> for pictures from this production<br><br>";
	}

	$media_select = mysql_query("select ID from media where IDtype='show' and assocID=$show_id");
	$do_media_check = mysql_num_rows($media_select);
	if($do_media_check > 0) {
	    echo "Click <a href=\"?action=media&amp;IDtype=show&amp;ID=$show_id\">here</a> to see associated media.\n";
	}

	if ($_SESSION['level'] >= 2 ) {
	echo <<< EOT4
	<form method=post action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='editshow'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type=submit value='Edit Show Details'>
	</form><br>
EOT4;
	}
	if ($_SESSION['level'] >= 3 ) {
	echo <<< EOT4
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addcast'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type=submit value='Add Cast to Show'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addcrew'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type=submit value='Add Crew to Show'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addawards'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type=submit value='Add Award to Show'>
	</form><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='show_detail'>
        <input type='hidden' name='show_id' value='$show_id'>
EOT4;
	if ($editP == 'edit') {
	    echo "	<input type=submit value='Hide Edit View'>\n";
	} else {
	    echo "	<input type='hidden' name='view' value='edit'>\n";
	    echo "	<input type=submit value='Show Edit View'>\n";
	}
        echo "	</form><br>\n";
        if ( ( isset ($_SESSION['username']) && ($_SESSION['username'] == $uname)) || ($_SESSION['level'] >= 5) ) {
            echo <<< EOT6
        <form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='delshow_check'>
        <input type='hidden' name='show_id' value='$show_id'>
        <input type=submit value='Delete this Show'>
        </form><br>
EOT6;
	}
	}
	echo <<< EOT4
	</td>
	<td valign=top align=center width=70%>
	<table border=0 cellspacing=0 cellpadding=0 width=100%>

EOT4;
    }
    if (strlen($funfacts) > 4 ) {
	echo "<tr align='center'><td colspan=3><b><u>Fun Facts</u></b><br><br>$funfacts<br><br></td></tr>\n";
    }
    if (strlen($toaster) > 4 ) {
	echo "<tr align='center'><td colspan=3><b><u>Where was the Toaster?</u></b><br><br>$toaster<br><br></td></tr>\n";
    }

    $award_select = mysql_query("select a.year,l.name,a.peepID,a.ID from awards a INNER JOIN awards_list l ON a.awardID=l.ID where a.showID=$show_id ORDER by l.ID");
    $do_award_check = mysql_num_rows($award_select);
    if($do_award_check > 0) {
	echo"<tr><td colspan=3 align=center><b><u>Awards</u></b><br><br></td></tr>\n";
	echo"<tr><td align=center colspan=3><table>\n";
	while ($result = mysql_fetch_array($award_select)) {
	    $award_title = stripslashes($result[1]);
	    $ID_to_edit = $result[3];
	    if ($result[2] > 0) {
		$fullname=get_fullname($result[2]);
		echo "	   <tr><td align=center><a href=\"?action=awards_by_year&amp;year=$result[0]\">$result[0]</a></td><td> &#160; - &#160; </td><td>$award_title</td><td> &#160; - &#160; </td><td align=left><a href=\"?action=peep_detail&amp;peep_id=$result[2]\">$fullname</a>";
	    } else {
		echo "	   <tr align=center><td align=center><a href=\"?action=awards_by_year&amp;year=$result[0]\">$result[0]</a></td><td> &#160; - &#160; </td><td align=left>$award_title";
	    }
	    if ($editP == 'edit') { echo"</td><td>";editbutton('editawards',$ID_to_edit); }
	    echo "</td></tr>\n";
	}
	echo"</table>\n";
    }

    $other_select = mysql_query("select ID,year from shows where title='$title' and ID!=$show_id");
    $other_check = mysql_num_rows($other_select);
    if ($other_check > 0) {
	echo "<tr align=center><td colspan=3><br><b><u>Other Performances</u></b><br><br>";
	while ($other_result = mysql_fetch_array($other_select)) {
	    echo "<a href=\"$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$other_result[0]\">$disptitle ($other_result[1])</a><br>\n";
	}
	echo "</td></tr>\n";
    }

    echo"<tr><td colspan=3 align=center><br><b><u>Cast</u></b><br><br></td></tr>\n";
    $cast_select = mysql_query("select c.role,c.peepID,c.ID from cast c JOIN people p on p.ID=c.peepID where c.showID=$show_id order by p.lname,p.fname");
    while ($result = mysql_fetch_array($cast_select)) {
	$role=stripslashes($result[0]);
	$peep_id=$result[1];
	$ID_to_edit = $result[2];
	$wholename = get_fullname($peep_id);
	echo "<tr valign=top ><td width=47% align=right><a href=\"$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=$peep_id\">$wholename</a></td><td width=6% align=center>&nbsp; - &nbsp;</td><td align=left>$role";
	if ($editP == 'edit') { editbutton('editcast',$ID_to_edit); }
	echo "</td></tr>\n";
    }
    echo"<tr><td colspan=3 align=center><br><br><b><u>Crew</u></b><br><br></td></tr>\n";

    $crew_select=mysql_query("select j.job,c.peepID,c.ID,j.URL from crew c JOIN jobs j ON j.ID=c.jobID where c.showID=$show_id order by j.priority");
    while ($result = mysql_fetch_array($crew_select)) {
	$job=stripslashes($result[0]);
	$peep_id=$result[1];
	$ID_to_edit=$result[2];
	$wholename = get_fullname($peep_id);
	$URL=stripslashes($result[3]);
	echo "<tr valign=top><td width=47% align=right><a href=\"$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=$peep_id\">$wholename</a></td><td width=6% align=center>&nbsp; - &nbsp;</td><td align=left>";
	if (strlen($URL) > 3 ) {
	    echo "<a target='new' href=\"$URL\">$job</a>";
	} else {
	    echo "$job";
	}
	if ($editP == 'edit') { editbutton('editcrew',$ID_to_edit,$show_id); }
	echo "</td></tr>\n";
    }
    echo "</table>\n";
    echo"</td></tr></table>\n";
}
?>
