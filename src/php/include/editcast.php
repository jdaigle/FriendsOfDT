<?

function editcast($cast_id,$show_id,$editP) {
    min_auth_level(2,'Not Authorized');
    check_number($cast_id);
    $cast_select = mysql_query("SELECT peepID,showID,role FROM cast WHERE ID=$cast_id");
    while ($result = mysql_fetch_array($cast_select)) {
	$peep_id = $result[0];
	if (!isset($show_id)) { $show_id = $result[1]; }
	$role =$result[2];
    }
    include_once ("include/get_title.php");
    $title = get_title($show_id);
    echo <<< EOT10
	<table align=center frame=1 width=50%>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle><td colspan=2 align=center>
	<font size=+2><b>Add/Edit Cast Member: $title</b></font>
	</td></tr><tr align=center valign=middle>
	<th align=center>Actor</th><th align=center>Role</th></tr>
	<tr><td align=center>
EOT10;
    include_once ("include/peep_option.php");
    peep_option($peep_id);
    echo <<< EOT10
	</td><td align=center><input type=text autocomplete="off" name=role value="$role" size=40></td></tr>
	<tr><td align=right>
	<input type='hidden' name='action' value='update_cast'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type='hidden' name='ID' value='$cast_id'>
EOT10;
    if (isset($editP) && $editP=='edit') {
	echo "	<input type='hidden' name='view' value='edit'>";
    }
    echo <<< EOT10
	<input type=submit value='Add/Edit Cast Member'>
	</form></td><form method=POST action='$_SERVER[PHP_SELF]'>
	<td align=left>
	<input type='hidden' name='action' value='delcast'>
	<input type='hidden' name='ID' value='$cast_id'>
	<input type=submit value='Delete Cast Record'>
	</td></tr></form></table>
EOT10;
}

function update_cast($ID,$editP) {
    checkpass();
    $peep_id = $_REQUEST['peep_id'];
    $show_id = $_REQUEST['show_id'];
    $role    = mysql_real_escape_string($_REQUEST['role']);
    mysql_query ("update cast set showID=$show_id, peepID=$peep_id, role='$role', last_mod=CURRENT_DATE where ID=$ID");
    echo <<< EOT9
    <p align=center>
	<b>Cast member added!</b><br>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='addcast'>
	<input type='hidden' name='show_id' value='$show_id'>
	<input type=submit value='Add Another?'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
    <p align=center>
	<input type='hidden' name='action' value='show_detail'>
	<input type='hidden' name='show_id' value='$show_id'>
EOT9;
    if (isset($editP) && $editP=='edit') {
	echo "	<input type='hidden' name='view' value='edit'>";
    }
    echo <<< EOT9
	<input type=submit value='All Done!'>
	</form>
EOT9;
}
?>
