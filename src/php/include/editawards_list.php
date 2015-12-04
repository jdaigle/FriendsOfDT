<?

function editawards_list($ID) {
    min_auth_level(4,'Not Authorized');
    $aw_query = mysql_query("select name from awards_list where ID=$ID");
    while ($awlist = mysql_fetch_array($aw_query)) {
	$name=$awlist[0];
    }
    echo <<< EOT10
	<table align=center frame=1 width=50%>
	<tr align=center valign=middle>
	<td colspan=2 align=center><font size=+2><b>Add/Edit an Award Listing</b></font></td>
	</tr><tr align=center valign=middle>
	<th colspan=2 align=center>Award Name</th></tr>
	<tr><td colspan=2 align=center>
	<form method=POST action='$_SERVER[PHP_SELF]'>
	<input type=text autocomplete="off" name=name value='$name' size=30></td></tr>
        <tr><td align=right> 
	<input type='hidden' name='action' value='update_awards_list'>
	<input type='hidden' name='ID' value='$ID'>
	<input type=submit value='Add/Edit Award'>
        </form></td>
        <td align=left> 
        <form method=POST action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='ID' value='$ID'>
        <input type='hidden' name='action' value='delawards_list'>
        <input type=submit value='Delete Award Listing'>
	</form></td></tr></table>
EOT10;
}

function update_awards_list($ID) {
    checkpass();
    min_auth_level(4,'Not Authorized');
    $name = mysql_real_escape_string($_REQUEST['name']);
    $insert = mysql_query ("update awards_list set name='$name' where ID=$ID");
    if(!$insert){ die("There is little problem: ".mysql_error()); }
    echo "<p align=center><font size=+2><b>Award Edited!</b></font><br><br>";
    admin_interface();
}
?>
