<?
function framework() {
    echo <<< EOT1
<html>
    <head>
	<link rel="shortcut icon" href="favicon.ico" >
    </head>
    <title>DramaTech Theatre Cast and Crew Database - IMDT</title>
    <body background=''>
    <table border=0 width=100% bgcolor='b6b6f2'>
	<tr>
	<td align=center valign=middle>
	    <a href="$_SERVER[PHP_SELF]"><img src='images/dtlogo.png'></a>
	</td>
	<td align=center valign=middle><a href="?"><img src='images/dttitle.png'></a></td>
	<td align=center valign=middle><a href="?"><img src='images/imdtlogo.png'></a><br>
EOT1;
    if(isset($_SESSION['username']) && isset($_SESSION['password'])){
	$username = mysql_real_escape_string($_SESSION['username']);
	$id_request = mysql_query("select ID from people where username='$username'");
	while ($id_result = mysql_fetch_array($id_request)) { $peep_id = $id_result[0]; }
	echo "<b>Welcome <a href=\"$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=$peep_id\">$username</a> </b>(<a href=\"$_SERVER[PHP_SELF]?action=logout\">Logout</a>";
	if ($_SESSION['level'] >= 4) {
	   echo " | <a href=$_SERVER[PHP_SELF]?action=admin>Admin Page</a>";
	}
	echo")";
    }else{
	$login_level=0;
	echo "Welcome Guest! ( <a href=\"$_SERVER[PHP_SELF]?action=loginpage\">Login</a> | <a href=\"$_SERVER[PHP_SELF]?action=register_form\">Register</a> )";
    }
    echo <<< EOT1
	</td></tr>
    </table>
    <basefont face=verdana>
    <hr>
    <table border='0' align=center>
	<tr border='0'><td>
    	<table border='0' align=center>
		<tr border='1'> <form method=GET action='$_SERVER[PHP_SELF]'> <td>
		<input type='hidden' name='action' value='shows_by_year'>
		<input type=submit value='Browse Shows by Year'></td>
		</form>
		<form method=GET action='$_SERVER[PHP_SELF]'> <td>
		<input type='hidden' name='action' value='shows_by_title'>
		<input type=submit value='Browse Shows by Title'></td>
		</form>
	</table></td><td>
    	<table border='0' align=center>
		<tr border='1'>
		<form method=GET action='$_SERVER[PHP_SELF]'>
		<td><b>Search: </b></td><td>
		<input type='hidden' name='action' value='search'>
		<input type='text' name='searchfield' size='40' value="$_REQUEST[searchfield]">
		<select name='searchtype'>
EOT1;
    echo "
		<option value='all'>All</option>
		<option value='show'" . ($_REQUEST['searchtype'] == "show" ? " selected" : "") . ">Shows</option>
		<option value='peep'" . ($_REQUEST['searchtype'] == "peep" ? " selected" : "") . ">People</option>";
    echo <<< EOT1
		</select>
		</td>
		<td align=center><input type=submit value='Search'></td>
		</form>
	</table>
	</table>
	<hr>
EOT1;
}
?>
