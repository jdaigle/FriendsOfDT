<?

function editEC_list($ID) {
    min_auth_level(4,'Not Authorized');
    $aw_query = mysql_query("select title from EC_list where ID=$ID");
    while ($awlist = mysql_fetch_array($aw_query)) {
	$name=$awlist[0];
    }
    echo <<< EOT10
	<table align=center frame=1 width=50%>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	<tr align=center valign=middle>
	<td colspan=2 align=center><font size=+2><b>Add/Edit an EC Title</b></font></td>
	</tr><tr align=center valign=middle>
	<th colspan=2 align=center>EC Position</th></tr>
	<tr><td colspan=2 align=center>
	<input type=text autocomplete="off" name=name value='$name' size=30></td></tr>
        <tr><td align=right> 
	<input type='hidden' name='action' value='update_EC_list'>
	<input type='hidden' name='ID' value='$ID'>
	<input type=submit value='Add/Edit position'>
        </td></form>
        <form method=POST action='$_SERVER[PHP_SELF]'>
        <td align=left> 
	<input type='hidden' name='ID' value='$ID'>
        <input type='hidden' name='action' value='delEC_list'>
        <input type=submit value='Delete EC Position'>
	</td></form></tr></table>
EOT10;
}

function update_EC_list($ID) {
    checkpass();
    min_auth_level(4,'Not Authorized');
    $name = mysql_real_escape_string($_REQUEST['name']);
    $insert = mysql_query ("update EC_list set title='$name' where ID=$ID");
    if(!$insert){ die("There is little problem: ".mysql_error()); }
    echo "<p align=center><font size=+2><b>EC Position Edited!</b></font><br><br>";
    include_once ("include/admin_framework.php");
    admin_framework();
}
?>
