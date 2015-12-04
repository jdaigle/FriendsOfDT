<?
function approval_page($approve_type) {
    if ($approve_type == 'new_users') {
	$approve_query = 'SELECT ID,username,email,level from people where username is not null and level=0 order by lname';
    } elseif ($approve_type == 'all_users') {
	$approve_query = 'SELECT ID,username,email,level from people where username is not null order by lname';
    } else {
	echo "Something funny going on here - Aborting.";
	endlines();
    }
    $approve_select = mysql_query($approve_query);
    if (mysql_num_rows($approve_select) < 1) {
	echo "<center><h1>No users to modify or approve!</h1></center>\n";
	endlines();
    }
    echo <<< EOT
	<table border=1 frame=1 align=center><tr><td>
<b>Authorization Level information:</b><br>

	<b>0</b> = no privilege<br>
	<b>1</b> = 0 + able to edit self<br>
	<b>2</b> = 1 + able to edit show/people details<br>
	<b>3</b> = 2 + able to associate extisting people with shows<br>
	<b>4</b> = 3 + able to create new records (people, shows, etc.)<br>
	<b>5</b> = 4 + able to modify users (Admin Privileges)<br>
	</td></tr></table><br>

	<table border=1 frame=1 align=center>
	<tr><td align=center colspan=6><font size=+2>Modify User Authorization Level</font></td></tr>
	<tr><th>&nbsp;</th>
	<th align=center>Full Name</th>
	<th align=center>User Name</th>
	<th align=center>Email Address</th>
	<th align=center>Auth Level</th>
	</tr>
EOT;
    $o=0;
    include_once ("include/get_fullname.php");
    while ($to_approve = mysql_fetch_array($approve_select)) {
	$peep_id = $to_approve[0];
	$newusername = stripslashes($to_approve[1]);
	$email = stripslashes($to_approve[2]);
	$curlevel = $to_approve[3];
	$fullname= get_fullname($peep_id);
	$o++;
	echo <<< EOT
	<tr>
	<td align=center>$o</td>
	<td align=center>$fullname</td>
	<td align=center>$newusername</td>
	<td align=center>$email</td>
	<td align=center>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='approve'>
	<input type='hidden' name='chtype' value='$approve_type'>
	<input type='hidden' name='peep_id' value='$peep_id'>

	<select name='new_level'>
EOT;
	for ( $i = 0; $i <= 5; $i += 1) {
	    echo "		<option ";
	    if ($curlevel == $i) { echo "selected "; }
	    echo "value='$i'>$i</option>\n";
	}
    echo <<< EOT
	</select>
	<input type=submit value='Approve'>
	</form>
	<form method=GET action='$_SERVER[PHP_SELF]'>
	<input type='hidden' name='action' value='user_fail'>
	<input type='hidden' name='chtype' value='$approve_type'>
	<input type='hidden' name='peep_id' value='$peep_id'>
	<input type=submit value='FAIL'>
	</form>
	</td></tr>
EOT;
    }
    echo "</table>\n";
}

function user_fail($approve_type) {
    $peep_id = $_REQUEST['peep_id'];
    mysql_query("update people set username=NULL, password=NULL, email=NULL, level=0 where ID=$peep_id");
    echo "<font size=+2 align=center>Bogus user removed for Person ID $peep_id</font><br><br>";
    $approve_type = $_REQUEST['chtype'];
    approval_page($approve_type);
}

function change_level() {
    $new_level = $_REQUEST['new_level'];
    check_number($new_level);
    $peep_id = $_REQUEST['peep_id'];
    $approve_type = $_REQUEST['chtype'];
    mysql_query("update people set level=$new_level where ID=$peep_id");
//send an e-mail
    $query = mysql_query("select email,username from people where ID=$peep_id");
    while ($result = mysql_fetch_array($query)) {
	$email = stripslashes($result[0]);
	$uname = stripslashes($result[1]);
    }
    include_once ("include/get_fullname.php");
    $fullname= get_fullname($peep_id);
    $to      = "toneman@rocketmail.com,$email";
    $subject = "IMDT user access confirmed";
#    $message = "$fullname has been granted access to <a href=\"http://imdt.vilafamily.com/\">IMDT</a>.  Welcome aboard!\n\nNow that you have access, you can do such things as update your bio and upload a headshot.  If you have requested and been granted advanced access, you will have additional buttons on each page enabling you to edit and add entries.  Thanks for joining IMDT!\n";
    $message = "$fullname has been granted level $new_level access to <a href=\"http://imdt.vilafamily.com/\">IMDT</a>.  Welcome aboard!\n\nWith your new level of access, you can do such things as: \n\n";
    if ($new_level >= 1) {
	$message = $message . "   * Edit your personal bio\n   * Upload a headshot\n";
    }
    if ($new_level >= 2) {
	$message = $message . "   * Edit details (i.e. upload pictures or edit bios) for other people or shows\n";
    }
    if ($new_level >= 3) {
	$message = $message . "   * Associate existing people with existing shows\n";
    }
    if ($new_level >= 4) {
	$message = $message . "   * Create new records for people and shows\n";
    }
    if ($new_level >= 5) {
	$message = $message . "   * Administer the application and databse\n";
    }
    $message = $message . "\nThanks for joining IMDT!\n\nTony Vila\nWebmaster, Creator of IMDT";
    $headers = "From: imdt@vilafamily.com\r\n" .
	"Reply-To: toneman@rocketmail.com\r\n" .
	"X-Mailer: PHP/" . phpversion();
    mail($to, $subject, $message, $headers);
    echo "<h1><center>User $uname updated, mail sent!</center></h1><br>\n";
    approval_page($approve_type);
}

?>
