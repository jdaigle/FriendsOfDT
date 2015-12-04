<?

function whats_new($days) {
    if (!isset($days)) { $days=7; }
    echo <<< EOTWN
    	<center><font size=+3>Changes in the last $days days</font><br>
        Want to check a different date range?<br>
        <form method=GET action='$_SERVER[PHP_SELF]'>
        <input type='hidden' name='action' value='whats_new'>
        Number of days back to search: <input type=text name=days value='$days' size=4>
        <input type=submit value='Search'>
        </form>
EOTWN;
    $peep_q = mysql_query("select DISTINCT(ID) from people where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod or ID in (select DISTINCT(peepID) from awards where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod) or ID in (select DISTINCT(peepID) from EC where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod) order by lname,fname");
    $rowcheck = mysql_num_rows($peep_q);
    if ($rowcheck > 0) {
	include_once ("include/get_fullname.php");
        echo "<br><br><font size=+2>People Added or Edited</font><br>";
        while ($peep_mod = mysql_fetch_array($peep_q)) {
            $name = get_fullname($peep_mod[0]);
            echo "<a href=\"$_SERVER[PHP_SELF]?action=peep_detail&amp;peep_id=$peep_mod[0]\">$name</a><br>";
        }
    }
    $show_q = mysql_query("select DISTINCT(ID) from shows where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod or ID in (select DISTINCT(showID) from cast where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod) or ID in (select DISTINCT(showID) from crew where DATE_SUB(CURDATE(),INTERVAL $days DAY) <= last_mod) order by year,title");
    $rowcheck = mysql_num_rows($show_q);
    if ($rowcheck > 0) {
	include_once ("include/get_title.php");
        echo "<br><br><font size=+2>Shows Added or Edited</font><br>";
        while ($show_mod = mysql_fetch_array($show_q)) {
            $name = get_title($show_mod[0]);
            echo "<a href=\"$_SERVER[PHP_SELF]?action=show_detail&amp;show_id=$show_mod[0]\">$name</a><br>";
        }
    }
    echo "</center>\n";
}

?>
