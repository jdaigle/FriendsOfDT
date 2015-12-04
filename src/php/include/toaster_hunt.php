<?
include_once ("include/get_q.php");
include_once ("include/rearrange_title.php");
function toaster_hunt() {
    echo "<br><center><font size=+3><b>What's With The Toaster?</b></font></center><br><br>";
    echo "The <a href='?action=peep_detail&peep_id=2104'>Dramatech Toaster</a> has been hidden in plain sight in every DramaTech show since the late 1980s. The idea for this came from an impromptu art installation called <a href='?action=media_detail&media_id=2985'><i>Toaster Dreams</i></a> created by <a href='?action=peep_detail&peep_id=334'>Eddie Maise</a> in the <a href='?action=media_detail&media_id=2553'>old theater</a>. It featured several pieces of furniture suspended from the grid, with a toaster spotlit in the middle.<br><br>This list is a humble attempt to record where the creative folk of DT have tried to sneak this icon into their shows.<br><br>";
    echo "<br><table align=center width=75% border=1>\n";
    echo "<tr><td colspan=3 align=center><font size=+2>Where was the Toaster?</font></td></tr>\n";
    $toast_select = mysql_query("select title,quarter,year,toaster,ID from shows where toaster is not null order by year,quarter");
    while ($result = mysql_fetch_array($toast_select)) {
        $title=stripslashes($result[0]);
        $disptitle=rearrange_title($title);
        $quarter=get_q($result[1]);
        $date="$quarter $result[2]";
        $toaster = str_replace("\n", "<br>", $result[3]);
	if (strlen($toaster) > 4 ) {
	    echo "<tr><td><a href='?action=show_detail&show_id=$result[4]'>$disptitle</a></td><td>$date</td><td>$toaster</td></tr>\n";
	}
    }
    echo "</table><br>";
}
?>
