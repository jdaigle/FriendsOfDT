<?


function loginpage(){
    echo <<< EOT9
	<br><br><br>
	<form action='$_SERVER[PHP_SELF]' method='post'>
	<input type='hidden' name='action' value='login'>
	<table align=center frame=1>
	<tr><td colspan=2 align=center><font size=+2>Login to IMDT</font></td></tr>
	<tr><td align=right>Username: </td>
	    <td align=center><input type='text' name='username' size='30'></td></tr>
	<tr><td align=right>Password: </td>
	    <td align=center><input type='password' name='password' size='30'></td></tr>
	<tr><td colspan=2 align=center><input type='submit' value='Login'></td></tr>
	<tr><td colspan=2 align=center>Forgot your password?  Please <a href="?action=register_form">re-register</a>.</td></tr>
	</table>
	</form>
EOT9;
}

function login(){
    $username = mysql_real_escape_string($_REQUEST['username']);
    $password = mysql_real_escape_string( md5($_REQUEST['password']) );
    $result = mysql_query("SELECT level FROM people WHERE username='$username' AND password='$password'");
    $row = mysql_fetch_array($result);
    if ($row != null){
    $_SESSION['username']= "$username";
    $_SESSION['password']= "$password";
    $_SESSION['level']= "$row[0]";
    framework();
    echo "<font size=+2><center>Welcome, ".$username."!</center></font>";
    }else {
	echo "<center><b>Bad Username $username or password $password.  Please try again.</b></center><br><br>\n";
	loginpage();
    }
}

function logout() {
    session_destroy();
    unset ($_SESSION['username']);
    unset ($_SESSION['password']);
    unset ($_SESSION['level']);
    framework();
    echo "<br><font size=+2><center>You are now logged out!</center></font>";
}

function register_form(){
    echo <<< EOT6
    <br><br>
    <form action='$_SERVER[PHP_SELF]' method='post'>
    <table align=center valign=middle frame=1>
    <tr><td align=right> Username: </td><td>
    <input type='text' name='username' size='30'></td></tr>
    <tr><td align=right> Password: </td><td>
    <input type='password' name='password' size='30'></td></tr>
    <tr><td align=right> Confirm Password: </td><td>
    <input type='password' name='password_conf' size='30'></td></tr>
    <tr><td align=right> E-mail Address: </td><td>
    <input type='text' name='email' size='30'></td></tr>
    <tr><td align=right>Who Are You? </td><td>
EOT6;
    include_once ("include/peep_option.php");
    peep_option(NULL);
//----Testing Captcha code start ----------//
    echo "</td></tr><tr><td colspan=2 align=center>";
    include_once("include/recaptchalib.php");
    $publickey = "6LcJDMkSAAAAADOmG6mBkI2keShAJnXn2b9ulIqp";
    echo recaptcha_get_html($publickey);
//----Testing Captcha code END ----------//
    echo <<< EOT6
    </td></tr><tr><td colspan=2 align=center>
    <input type='hidden' name='action' value='register'>
    <input type='submit' value='Register'>
    </td></tr></table>
    </form>
    <br><br>
    <center>Currently, the main benefit to registration is the ability to edit your personal info.<br>
    As such, we are only allowing people to register who are already in the database.<br>
    If you need to be added to the database, please
    <a href="mailto:fodtadmin@cridion.com?subject=Please add me to IMDT">contact the webmaster</a>.</center><br>
EOT6;
}

function register(){
    $username = mysql_real_escape_string($_REQUEST['username']);
    $password = mysql_real_escape_string($_REQUEST['password']);
    $pass_conf = mysql_real_escape_string($_REQUEST['password_conf']);
    $email = mysql_real_escape_string($_REQUEST['email']);
    $peep_id = $_REQUEST['peep_id'];

    if(empty($username)){ echo "Please enter your username!<br><br>"; register_form(); endlines();}
    if(empty($password)){ echo "Please enter your password!<br>"; register_form(); endlines();}
    if(empty($pass_conf)){ echo "Please confirm your password!<br>"; register_form(); endlines();}
    if(empty($email)){ echo "Please enter your email!"; register_form(); endlines();}
    if(empty($peep_id)){ echo "Please choose a DT member to associate with!"; register_form(); endlines();}

    $user_check = mysql_query("SELECT username FROM users WHERE username='$username'");
    $do_user_check = mysql_num_rows($user_check);
    if($do_user_check > 0){ echo "Username is already in use!<br>"; register_form(); endlines();}

    $email_check = mysql_query("SELECT email FROM users WHERE email='$email'");
    $do_email_check = mysql_num_rows($email_check);
    if($do_email_check > 0){ echo "Email is already in use!"; register_form(); endlines();}

    if($password != $pass_conf){ echo "Passwords don't match!"; register_form(); endlines();}
    //Check Captcha code
    require_once('recaptchalib.php');
    $privatekey = "6LcJDMkSAAAAANxjo1sOpDZ08jITw0MJ2k7DADop";
    $resp = recaptcha_check_answer ($privatekey,
                                    $_SERVER["REMOTE_ADDR"],
                                    $_POST["recaptcha_challenge_field"],
                                    $_POST["recaptcha_response_field"]);
    if (!$resp->is_valid) { echo "CAPTCHA code does not match. Please try again!"; register_form(); endlines();}

    //If everything is okay let's register this user
    $insert = mysql_query("UPDATE people set level=0, username='$username', password=md5('$password'), email='$email' where ID=$peep_id");
    if(!$insert){ die("There's little problem: ".mysql_error()); }
    echo <<< EOT1
	<center>$username, you are now registered. Thank you!<br>
	You will have no privileges until the webmaster verifies your registration and association to DT.
	Once you are set up, you will receive an e-mail.
	You may continue to browse the site with guest-level privileges until then.</center>
EOT1;
    include_once ("include/get_fullname.php");
    $fullname= get_fullname($peep_id);
    $to      = "fodtadmin@cridion.com";
    $subject = "New Member Added to IMDT";
    $message = "Please confirm member $username is associated to $fullname\n";
    $headers = "From: fodtadmin@cridion.com\r\n" .
	"Reply-To: fodtadmin@cridion.com\r\n" .
	"X-Mailer: PHP/" . phpversion();
    mail($to, $subject, $message, $headers);
}

?>
