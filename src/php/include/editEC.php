<?
function editEC($ID,$peep_id) {
    min_auth_level(2,'Not Authorized');
    $list = mysql_query("select peepID,ECID,year from EC where ID=$ID");
    while ($result = mysql_fetch_array($list)) {
	if (!isset($peep_id)) { $peep_id = $result[0]; }
	$cur_ECID = $result[1];
	$year = $result[2];
    }
    echo <<< EOT10
	<table align=center frame=1 width=50%>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle><td colspan=3 align=center>
	<font size=+2><b>Add/Edit Person Executive Council Record</b></font>
	</td></tr><tr align=center valign=middle>
	<th align=center>Person</th><th align=center>Position</th><th align=center>Year</th></tr>
	<tr><td align=center>
EOT10;
    include_once ("include/peep_option.php");
    peep_option($peep_id);
    echo "</td><td align=center><select name='EC_id'><option value=''></option>\n";
    $EC_select = mysql_query("SELECT ID,title FROM EC_list ORDER BY ID");
    while ($EClist = mysql_fetch_array($EC_select)) {
	$EC_id=$EClist[0];
	$title=stripslashes($EClist[1]);
	echo "	<option value='$EC_id'";
	if ($cur_ECID == $EC_id) { echo " selected"; }
	echo ">$title</option>\n";
    }
    echo <<< EOT10
	</td><td align=center><input type=text autocomplete="off" name=year value='$year' size=4></td></tr>
	<tr><td align=right>
	<input type='hidden' name='action' value='updateEC'>
	<input type='hidden' name='ID' value='$ID'>
	<input type=submit value='Add/Edit EC Member'>
	</td></form><form method=POST action='$_SERVER[PHP_SELF]'>
	<td align=left rowspan=2>
	<input type='hidden' name='ID' value='$ID'>
	<input type='hidden' name='action' value='delEC'>
	<input type=submit value='Delete EC Member'>
	</td></tr>
	</form></table>
EOT10;
}

function updateEC() {
    min_auth_level(2,'Not Authorized');
    checkpass();
    $peep_id = $_REQUEST['peep_id'];
    $EC_id = $_REQUEST['EC_id'];
    $year = $_REQUEST['year'];
    $ID = $_REQUEST['ID'];
    check_number($year);
    $insert = mysql_query ("update EC set peepID=$peep_id, ECID=$EC_id, year=$year, last_mod=CURRENT_DATE where ID=$ID");
    if(!$insert){ die("There's little problem: ".mysql_error()); }
    echo <<< EOT9
    <p align=center>
	<b>EC member added!</b><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='addEC'>
	<input type=submit value='Add Another?'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='peep_detail'>
	<input type='hidden' name='peep_id' value='$peep_id'>
	<input type=submit value='All Done!'>
	</form>
EOT9;
}
?>
