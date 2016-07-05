<?
function update_picture() {
    checkpass();
    min_auth_level(1,'Not Authorized');
    if (isset($_REQUEST['peep_id'])) {
	$peep_id=$_REQUEST['peep_id'];
	check_number($peep_id);
    } 
    if (isset($_REQUEST['show_id'])) {
	$show_id=$_REQUEST['show_id'];
	check_number($show_id);
    }
    if (!isset($peep_id) && !isset($show_id)) {
	echo "<center><b>Trying to add a picture with no association!  Please go back and try again.</b></center>";
	endlines();
    }
    $media_id = upload_image();
    $newname = "./media/$media_id.jpg";
    $mailname = "$media_id.jpg";
    $mailpath = "./media/$media_id.jpg";
    $thumbname = "./media/$media_id-thumb.jpg";
    $indname = "./media/$media_id-index.jpg";
    exec(sprintf("convert %s -resize 240x240 +profile \"\*\" %s", $newname, $thumbname));
    exec(sprintf("convert %s -resize 50x50 +profile \"\*\" %s", $newname, $indname));
    $update = mysql_query("update media_items set item='$newname', thumb='$thumbname', tiny='$indname' where ID=$media_id");
    if(!$update){ die("There is little problem: ".mysql_error()); }
    if ($show_id > 0) {
	$media_update_q = "insert into media set assocID=$show_id, IDtype='shows', item_id=$media_id, last_mod=CURRENT_DATE";
	$update = mysql_query ("$media_update_q");
	if(!$update){ die("There is little problem: ".mysql_error()); }
	include_once ("include/rearrange_title.php");
	$title_q=mysql_query("select title from shows where ID=$show_id");
	while ($title_r = mysql_fetch_array($title_q)) { $showname=$title_r[0]; }
	$showname=get_fullname($show_id);
	$showname = stripslashes($showname);
	$medianame = rearrange_title($showname);
    }
    if ($peep_id>0) {
	$media_update_q = "insert into media set assocID=$peep_id, IDtype='people', item_id=$media_id, last_mod=CURRENT_DATE";
	$update = mysql_query ("$media_update_q");
	if(!$update){ die("There is little problem (update picture): ".mysql_error()); }
	include_once ("include/get_fullname.php");
	$medianame=get_fullname($peep_id);
    }
	#Post the new media to Facebook!
	//mail_media($medianame, $mailpath);
    echo "Changes made!  The result should be visible below.  If not, please hit reload.<br>";
    include_once ("include/media.php");
    media_detail(NULL,$media_id); 
}

function upload_image() {
    //Check that we have a file
    if((!empty($_FILES["uploaded_file"])) && ($_FILES['uploaded_file']['error'] == 0)) {
	//Check if the file is JPG image and it's size is less than 1.5Mb
	$filename = basename($_FILES['uploaded_file']['name']);
	$ext = substr($filename, strrpos($filename, '.') + 1);
	if (preg_match("/jpg/i", $ext) || preg_match("/jpeg/i", $ext)) {
	    $junk = $_FILES["uploaded_file"]["type"];
	    if (preg_match("/image\/.*jpeg/", $junk)) {
		$size = $_FILES["uploaded_file"]["size"];
		if ($size < 1024*1024*1.5) {
		    $id=add_media_row();
		    $newname = "./media/$id.jpg";
		    //Attempt to move the uploaded file to it's new place
		    if ((move_uploaded_file($_FILES['uploaded_file']['tmp_name'],$newname))) {
			return($id);
		    } else { echo "Error: A problem occurred during file upload!"; endlines();}
		} else { echo "Error: File is too big ($size)! Only  images under 1.5Mb are accepted for upload"; endlines();}
	    } else { echo "Error: Bad Filetype ($junk)!  Only JPEG images are accepted for upload"; endlines();}
	} else { echo "Error: Bad Extension ($ext)!  Extension of the image file must be JPG, jpg, JPEG, or jpeg."; endlines();}
    } else { echo "Error: No file uploaded"; endlines();}
}

function add_media_row() {
    $insert = mysql_query ("insert into media_items (item, thumb, tiny) values ('','','')");
    if(!$insert){ die("There is little problem (add_media_row): ".mysql_error()); }
    $new_id_query = mysql_query ("select max(ID) from media_items");
    while ($new_id = mysql_fetch_array($new_id_query)) { $id=$new_id[0]; }
    return($id);
}

function mail_media($person, $mailpath) {

	require("class.phpmailer.php");
	//Variables Declaration
	$name = "IMDT";
	$email_subject = "Newly uploaded media for $person";
	$Email_msg ="This is the message Text :\n";
	$Email_to = "sky295soggy@m.facebook.com";
	$email_from = "imdt@vilafamily.com";
	$imagefile = $mailpath;

	$mail = new PHPMailer();
	
	$mail->IsSMTP();// send via SMTP
	$mail->Host     = "localhost"; // SMTP servers
	$mail->SMTPAuth = false; // turn on/off SMTP authentication
	
	$mail->From     = $email_from;
	$mail->FromName = $name;
	$mail->AddAddress($Email_to); 
	$mail->AddReplyTo($email_from);
	$mail->WordWrap = 50;// set word wrap

	$mail->AddAttachment($imagefile);
	$mail->Body = $Email_msg."Name : ".$name."\n";
							
	$mail->IsHTML(false);// send as HTML
	$mail->Subject  =  $email_subject;
	if(!$mail->Send())
	{
	   echo "Message was not sent <p>";
	   echo "Mailer Error: " . $mail->ErrorInfo;
	}
}

?>
