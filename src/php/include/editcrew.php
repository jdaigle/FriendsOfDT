<?

function editcrew($crew_id,$show_id,$editP) {
    min_auth_level(2,'Not Authorized');
    check_number($crew_id);
    $crew_select = mysql_query("SELECT peepID,showID,jobID FROM crew WHERE ID=$crew_id");
    while ($result = mysql_fetch_array($crew_select)) {
	$peep_id = $result[0];
	if (!isset($show_id)) { $show_id = $result[1]; }
	 $job_id = $result[2];
    }
    include_once ("include/get_title.php");
    $title = get_title($show_id);
    echo <<< EOT10
	<table align=center frame=1 width=50%>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle><td colspan=3 align=center>
	<font size=+2><b>Add Crew Member to: $title</b></font>
	</td></tr><tr align=center valign=middle>
	<th align=center>Crew Member</th><th align=center>Title</th></tr>
	<tr><td align=center>
EOT10;
    include_once ("include/peep_option.php");
    peep_option($peep_id);
    echo "</td><td align=center>";
    include_once ("include/job_option.php");
    job_option($job_id);
    echo <<< EOT20
	</td></tr>
	<tr><td align=right>
	<input type='hidden' name='action' value='update_crew'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type='hidden' name='ID' value='$crew_id'>
EOT20;
    if (isset($editP) && $editP=='edit') {
	echo "	<input type='hidden' name='view' value='edit'>";
    }
    echo <<< EOT20
	<input type=submit value='Add/Edit Crew Record'>
	</td></form><form method=POST action='$_SERVER[PHP_SELF]'>
	<td align=left>
	<input type='hidden' name='action' value='delcrew'>
	<input type='hidden' name='ID' value='$crew_id'>
	<input type=submit value='Delete Crew Record'>
	</tr></form></table>
EOT20;
}


function update_crew($show_id,$peep_id,$job_id,$ID,$editP) {
    checkpass();
    mysql_query ("update crew set showID=$show_id, peepID=$peep_id, jobID='$job_id', last_mod=CURRENT_DATE where ID=$ID");
    echo <<< EOT9
    <p align=center>
	<b>Crew member added!</b><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='addcrew'>
	<input type='hidden' name='show_id' value='$show_id'>
    <p align=center>
	<input type=submit value='Add Another?'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='show_detail'>
	<input type='hidden' name='show_id' value='$show_id'>
EOT9;
    if (isset($editP) && $editP=='edit') {
	echo "	<input type='hidden' name='view' value='edit'>";
    }
    echo <<< EOT9
    <p align=center>
	<input type=submit value='All Done!'>
	</form>
EOT9;
}

?>
