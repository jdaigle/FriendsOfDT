<?

function editawards($ID,$peep_id,$show_id) {
    min_auth_level(2,'Not Authorized');
    $aw_query = mysql_query("select showID,peepID,awardID,year from awards where ID=$ID");
    while ($awlist = mysql_fetch_array($aw_query)) {
	if (!isset($show_id)) {$show_id=$awlist[0];}
	if (!isset($peep_id)) {$peep_id=$awlist[1];}
	$cur_award_id=$awlist[2];
	$year=$awlist[3];
    }
    echo <<< EOT10
	<font size=+2><b>Add/Edit an Award in the Database</b></font>
	<table align=center frame=1 width=50%>
	<tr align=center valign=middle><td colspan=4 align=center>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	</td></tr><tr align=center valign=middle>
	<th align=center>Person</th>
	<th align=center>Show</th>
	<th align=center>Award</th>
	<th align=center>Year</th></tr>
	<tr><td align=center>
EOT10;
    include_once ("include/peep_option.php");
    peep_option($peep_id);
    echo "</td><td align=center>";
    include_once ("include/show_option.php");
    show_option($show_id);
    echo "</td><td align=center><select name='award_id'><option value=''></option>\n";
    $award_select = mysql_query("SELECT ID,name FROM awards_list ORDER BY ID");
    while ($awardlist = mysql_fetch_array($award_select)) {
	$award_id=$awardlist[0];
	$title=stripslashes($awardlist[1]);
	echo "	<option value='$award_id'";
	if ($award_id == $cur_award_id) { echo " selected";}
	echo ">$title</option>\n";
    }
    echo <<< EOT10
	</td><td align=center><input type=text autocomplete="off" name=year value='$year' size=4></td></tr>
        <tr><td colspan=2 align=right> 
	<input type='hidden' name='action' value='update_award'>
	<input type='hidden' name='ID' value='$ID'>
	<input type=submit value='Add/Edit Award Listing'>
        </form></td>
        <form method=POST action='$_SERVER[PHP_SELF]'>
        <td colspan=2 align=left> 
	<input type='hidden' name='ID' value='$ID'>
        <input type='hidden' name='action' value='delawards'>
        <input type=submit value='Delete Award Listing'>
	</td></form></tr></table>
EOT10;
}

function update_award() {
    checkpass();
    min_auth_level(2,'Not Authorized');
    $peep_id = $_REQUEST['peep_id'];
    $show_id = $_REQUEST['show_id'];
    $award_id = $_REQUEST['award_id'];
    $year = $_REQUEST['year'];
    $ID = $_REQUEST['ID'];
    if ($show_id > 0) {$action="show_detail"; $idtype="show_id"; $id=$show_id;}
    if ($peep_id > 0) {$action="peep_detail"; $idtype="peep_id"; $id=$peep_id;}
    if ($peep_id == '') {$peep_id="null";}
    if ($show_id == '') {$show_id="null";}
    check_number($year);
    $insert = mysql_query ("update awards set showID=$show_id, peepID=$peep_id, awardID=$award_id, year=$year, last_mod=CURRENT_DATE where ID=$ID");
    if(!$insert){ die("There's little problem: ".mysql_error()); }
    echo <<< EOT9
    <p align=center>
	<b>Award added!</b><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='addawards'>
	<input type=submit value='Add Another?'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='$action'>
	<input type='hidden' name='$idtype' value='$id'>
	<input type=submit value='All Done!'>
	</form>
EOT9;
}

?>
