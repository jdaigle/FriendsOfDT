<?

include_once ("include/get_fullname.php");
include_once ("include/get_pic.php");
include_once ("include/castcrew.php");

function detail_page($peep_id) {
    $peep_select = mysql_query("select media_id,bio from people where ID=$peep_id");
    while ($result = mysql_fetch_array($peep_select)) {
        $media_id = $result[0];
        $bio = str_replace("\n", "<br>", $result[1]);
    }
    $fullname= get_fullname($peep_id);
    $imagelink=get_thumb_link('people',$peep_id);
    
    echo <<< EOT33
        <table align=center>
        <tr><td>$imagelink</td>
        <td colspan=4><center><font size=+4>$fullname</font><br>
EOT33;
    if (strlen($bio) > 4) {
        echo"<br><b>Biographical Data</b><br><br> $bio<br><br>\n";
    }
    echo "</td></tr>";
    castcrew($peep_id);
    echo "<br><br><table border=0 frame=0 align=center><tr></table>\n";
}
?>
