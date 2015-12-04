<?

session_start();
include "dtcon.php";
dbcon();

global $_REQUEST;
global $_SESSION;
if (isset($_REQUEST['peep_id'])) {$peep_id = $_REQUEST['peep_id']; check_number($peep_id); }
if (isset($_REQUEST['media_id'])) {$media_id = $_REQUEST['media_id']; check_number($media_id); }
if (isset($_REQUEST['item_id'])) {$item_id = $_REQUEST['item_id']; check_number($item_id); }
if (isset($_REQUEST['days'])) {$days = $_REQUEST['days']; check_number($days); }
if (isset($_REQUEST['show_id'])) {$show_id = $_REQUEST['show_id']; check_number($show_id); }
if (isset($_REQUEST['job_id'])) {$job_id = $_REQUEST['job_id']; check_number($job_id); }
if (isset($_REQUEST['edit_job_id'])) {$edit_job_id = $_REQUEST['edit_job_id']; check_number($edit_job_id); }
if (isset($_REQUEST['ID'])) {$ID = $_REQUEST['ID']; check_number($ID); }
if (isset($_REQUEST['year'])) {$year = $_REQUEST['year']; check_number($year); }
if (isset($_REQUEST['view'])) {$editP = $_REQUEST['view']; }
if (isset($_REQUEST['IDtype'])) {$IDtype = $_REQUEST['IDtype']; }

echo <<< EOT1
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html xmlns="http://www.w3.org/1999/xhtml"
      xmlns:og="http://ogp.me/ns#"
      xmlns:fb="https://www.facebook.com/2008/fbml">
  <head>
    <script type="text/javascript" src="https://apis.google.com/js/plusone.js"></script>

    <title>DramaTech Theatre Cast and Crew Database - IMDT</title>
    <link rel="shortcut icon" href="favicon.ico" >
    <link rel="image_src" href="http://imdt.vilafamily.com/images/imdtlogo.png">
    <link type="text/css" rel="stylesheet" href="dtstyle.css">
    <meta property="og:title" content="IMDT - DramaTech Historical Database"/>
    <meta property="og:type" content="website"/>
    <meta property="og:url" content="http://imdt.vilafamily.com/"/>
    <meta property="og:image" content="http://imdt.vilafamily.com/images/imdtlogo.png"/>
    <meta property="og:site_name" content="IMDT"/>
    <meta property="fb:admins" content="tonyvila6"/>
    <meta property="og:description"
          content="IMDT provides historical information 
		   about DramaTech Theatre, Georgia Tech's 
		   student-run theatre and Atlanta's oldest 
		   continually operating theatre."/>

    </head>
    <body>
EOT1;

if (isset($_REQUEST['action'])) {
    $action = $_REQUEST['action'];
    switch($action){
	case 'login': include ("include/loginstuff.php"); login(); break;
	case 'loginpage': framework(); include ("include/loginstuff.php"); loginpage(); break;
	case 'logout': include ("include/loginstuff.php"); logout(); break;
	case 'register_form': framework(); include ("include/loginstuff.php"); register_form(); break;
	case 'register': framework(); include ("include/loginstuff.php"); register(); break;
	case 'shows_by_year': framework(); include ("include/browse.php"); browse_shows('year'); break;
	case 'shows_by_title': framework(); include ("include/browse.php"); browse_shows('title'); break;
	case 'approval_page': framework(); include ("include/approval.php"); approval_page('new_users'); break;
	case 'chlevel_page': framework(); include ("include/approval.php"); approval_page('all_users'); break;
	case 'approve': framework(); include ("include/approval.php"); change_level(); break;
	case 'user_fail': framework(); include ("include/approval.php"); user_fail(); break;
	case 'search': framework(); include ("include/search.php"); get_search_list(); break;
	case 'show_detail': framework(); include("include/show_detail.php"); show_show_detail($show_id,$editP); break;
	case 'peep_detail': framework(); include ("include/peep_detail.php"); show_peep_detail($peep_id,$editP); break;
	case 'admin': framework(); include ("include/admin_interface.php"); admin_framework(); break;
	case 'detail_page': include ("include/detail_page.php"); detail_page($peep_id); break;
	case 'addpeep': framework(); include ("include/add_del.php"); additem('people',4); break;
	case 'editpeep': framework(); include ("include/editpeople.php"); editpeople($peep_id); break;
	case 'update_peep': framework(); include ("include/editpeople.php"); update_peep(); break;
	case 'addshow': framework(); include ("include/add_del.php"); additem('shows',4); break;
	case 'editshow': framework(); include ("include/editshows.php"); editshows($show_id); break;
	case 'update_show': framework(); include ("include/editshows.php"); update_show(); break;
	case 'addjobs': framework(); include ("include/add_del.php"); additem('jobs',4); break;
	case 'editjobs': framework(); include ("include/editjobs.php"); editjobs($job_id); break;
	case 'update_job': framework(); include ("include/editjobs.php"); update_job($edit_job_id,$job_id); break;
	case 'addawards': framework(); include ("include/add_del.php"); additem('awards',3,$peep_id,$show_id); break;
	case 'editawards': framework(); include ("include/editawards.php"); editawards($ID); break;
	case 'update_award': framework(); include ("include/editawards.php"); update_award(); break;
	case 'awards_by_year': framework(); include ("include/awards_by_year.php"); awards_by_year($year); break;
	case 'EC_by_year': framework(); include ("include/EC_by_year.php"); EC_by_year($year); break;
	case 'delawards': framework(); include ("include/add_del.php"); delitem('awards',$ID); break;
	case 'addEC': framework(); include ("include/add_del.php"); additem('EC',3,$peep_id); break;
	case 'editEC': framework(); include ("include/editEC.php"); editEC($ID); break;
	case 'updateEC': framework(); include ("include/editEC.php"); updateEC(); break;
	case 'delEC': framework(); include ("include/add_del.php"); delitem('EC',$ID); break;
	case 'addcast': framework(); include ("include/add_del.php"); additem('cast',3,$show_id); break;
	case 'editcast': framework(); include ("include/editcast.php"); editcast($ID,$show_id,$editP); break;
	case 'update_cast': framework(); include ("include/editcast.php"); update_cast($ID,$editP); break;
	case 'delcast': framework(); include ("include/add_del.php"); delitem('cast',$ID); break;
	case 'addcrew': framework(); include ("include/add_del.php"); additem('crew',3,$show_id); break;
	case 'editcrew': framework(); include ("include/editcrew.php"); editcrew($ID,$show_id,$editP); break;
	case 'update_crew': framework(); include ("include/editcrew.php"); update_crew($show_id,$peep_id,$job_id,$ID,$editP); break;
	case 'delcrew': framework(); include ("include/add_del.php"); delitem('crew',$ID); break;
	case 'addawards_list': framework(); include ("include/add_del.php"); additem('awards_list',4); break;
	case 'editawards_list': framework(); include ("include/editawards_list.php"); editawards_list($ID); break;
	case 'update_awards_list': framework(); include ("include/editawards_list.php"); update_awards_list($ID); break;
	case 'delawards_list': framework(); include ("include/add_del.php"); delitem('awards_list',$ID); break;
	case 'addEC_list': framework(); include ("include/add_del.php"); additem('EC_list',4); break;
	case 'editEC_list': framework(); include ("include/editEC_list.php"); editEC_list($ID); break;
	case 'update_EC_list': framework(); include ("include/editEC_list.php"); update_EC_list($ID); break;
	case 'delEC_list': framework(); include ("include/add_del.php"); delitem('EC_list',$ID); break;
	case 'update_picture': framework(); include ("include/picstuff.php"); update_picture($pic_name); break;
	case 'whats_new': framework(); include("include/whats_new.php"); whats_new($days); break;
	case 'six_degrees': framework(); include("include/six_degrees.php"); six_degrees(); break;
	case 'stats': framework(); include("include/stats.php"); stats(); break;
	case 'dbcleanup_blanks': framework(); include("include/dbcleanup.php"); dbcleanup_blanks(); break;
	case 'ECbrowse': framework(); include("include/ECbrowse.php"); ECbrowse(); break;
	case 'delpeep_check': framework(); include ("include/delpeep_check.php"); delpeep_check($peep_id); break;
	case 'delperson': framework(); include ("include/add_del.php"); delitem('people',$ID); break;
	case 'delshow_check': framework(); include ("include/delshow_check.php"); delshow_check($show_id); break;
	case 'delshow': framework(); include ("include/add_del.php"); delitem('shows',$ID); break;
	case 'media_index': framework(); include ("include/media.php"); media_index($ID,$IDtype); break;
	case 'media_detail': framework(); include ("include/media.php"); media_detail($media_id,$item_id); break;
	case 'tie_media': framework(); include ("include/media.php"); tie_media($item_id,$peep_id,$show_id); break;
	case 'media_upload': framework(); include ("include/media.php"); media_upload($IDtype,$ID); break;
	case 'def_pic_change': framework(); include ("include/media.php"); def_pic_change($media_id,$IDtype,$ID); break;
	case 'toaster_hunt': framework(); include ("include/toaster_hunt.php"); toaster_hunt(); break;
	default: framework(); include("include/splashpage.php"); splashpage(); break;
    }
} else {
    framework(); include("include/splashpage.php"); splashpage();
}

endlines();


//Universal functions.

function editbutton($action,$ID) {
	echo "</td><td><a href=\"$_SERVER[PHP_SELF]?action=$action&amp;ID=$ID&amp;view=edit\">Edit</a></td></tr>\n";
}

function min_auth_level($auth_level,$errmsg) {
    if ($_SESSION['level'] < $auth_level) {
	echo "<center><font size=+3>$errmsg<font></center>\n";
	endlines();
    }
}

function check_number($x) {
    //Verify that what we think are numbers are in fact numbers.  Helps avoid SQL Injection Attacks.
    if (isset($x)){
	if ( preg_match ("/^\d*$/", $x) ) {
	    return;
	} else {
	    echo "Nice try, you dirty hacker!  Take your SQL injection elsewhere!";
	    endlines();
	}
    } else {
        return;
    }
}

function endlines() {
    echo "<table align=center><tr><td align=center valign=middle>Copyright 2011 Friends of Dramatech</td>";
    echo "</tr></table>\n";

    echo "\n</body>\n</html>\n";
    mysql_close();
    exit();
}

function checkpass() {
//    $adminpass = $_REQUEST['adminpass'];
    $sessuname = mysql_real_escape_string($_SESSION['username']); 
    $sesspass = mysql_real_escape_string($_SESSION['password']); 
    #obtain new row's ID
    $passwd_query = mysql_query ("select password from people where username='$sessuname'");
    while ($passwd = mysql_fetch_array($passwd_query)) {
	$dbpass=$passwd[0];
    }
    
//    if ($adminpass != '3ng!n33r5T43@+r3') {
    if ($dbpass != $sesspass) {
	echo "<h1>Invalid Password!  No changes made!</h1>";
	endlines();
    }
}
function framework() {
    //session_start();
    echo <<< EOT1
    <table border=0 width=100% bgcolor='#b6b6f2'>
	<tr>
	<td align=center valign=middle>
	    <a href="$_SERVER[PHP_SELF]"><img src='images/dtlogo.png'></a>
	</td>
	<td align=center valign=middle><img src='images/dttitle.png' border=0 USEMAP='#toaster'>
	<MAP NAME='toaster'>
	    <AREA SHAPE='RECT' COORDS='261,7,351,92' HREF='$_SERVER[PHP_SELF]?action=toaster_hunt'>
	    <AREA SHAPE='RECT' COORDS='64,0,381,136' HREF='$_SERVER[PHP_SELF]'>
	</MAP>
	</td>
	<td align=center valign=middle><a href="$_SERVER[PHP_SELF]"><img src='images/imdtlogo.png'></a><br>
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
	<br>
	</td></tr>
    </table>
    <basefont face=verdana>
    <table border=0 width=100%>
	<tr><td align=center valign='middle'>
	<br><b><a title="See what has changed in the last few days." href="?action=whats_new">Latest Updates</a></b>
	</td><td align=center valign='middle'>
		<br><b><a title="Top Ten Lists, Quick Show Lists, and more!" href="?action=stats">IMDT Stats</a></b>
	</td><td rowspan=2 align=center valign='middle'>
		<form method=GET action='$_SERVER[PHP_SELF]' STYLE="DISPLAY:INLINE;">
		<b>Search: </b>
		<input type='hidden' name='action' value='search'>
EOT1;
    if (isset($_REQUEST['searchtype'])) {
	echo "
		<input type='text' name='searchfield' size='40' value=\"$_REQUEST[searchfield]\">
		<select name='searchtype'>
		<option value='all'>All</option>
		<option value='show'" . ($_REQUEST['searchtype'] == "show" ? " selected" : "") . ">Shows</option>
		<option value='peep'" . ($_REQUEST['searchtype'] == "peep" ? " selected" : "") . ">People</option>";
    } else {
	echo "
		<input type='text' name='searchfield' size='40' value=''>
		<select name='searchtype'>
		<option value='all'>All</option>
		<option value='show'>Shows</option>
		<option value='peep'>People</option>";
    }
    echo <<< EOT1
		</select>
		<input type=submit value='Search'>
	</form>
	</td>
	<td rowspan=2 align=center valign='middle'>
	<br>
	<iframe src="https://www.facebook.com/plugins/like.php?href=http://imdt.vilafamily.com"
           scrolling="no" frameborder="0"
           style="border:none; width:300px; height:100px;">
	</iframe></td><td  align=center rowspan=2 valign=middle> <g:plusone></g:plusone>
	</td></tr>
	<tr><td align=center valign=middle>
	<b><a title="Show pictures, posters, and programs" href="?action=media_index">Media Center</a></b></td>
	<td align=center valign='middle'>
	<b><a title="How are people connected to each other?" href="?action=six_degrees">6 Degrees of DT</a></b></td></tr>
	</table>
	<hr>
EOT1;
}
// End Universal Functions

?>
